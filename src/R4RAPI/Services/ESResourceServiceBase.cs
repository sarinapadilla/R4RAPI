using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using R4RAPI.Models;


namespace R4RAPI.Services
{

    /// <summary>
    /// Base class for all ElasticSearch based ResourceServices
    /// </summary>
    public abstract class ESResourceServiceBase
    {
        /// <summary>
        /// The elasticsearch client
        /// </summary>
        protected readonly IElasticClient _elasticClient;

        /// <summary>
        /// The API options.
        /// </summary>
        protected readonly R4RAPIOptions _apiOptions;

        /// <summary>
        /// A logger to use for logging
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceServiceBase"/> class.
        /// </summary>
        /// <param name="client">An instance of a <see cref="T:Nest.ElasticClient"/>Client.</param>
        /// <param name="apiOptionsAccessor">API options accessor.</param>
        /// <param name="logger">Logger.</param>
        public ESResourceServiceBase(IElasticClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger logger)
        {
            this._elasticClient = client;
            this._apiOptions = apiOptionsAccessor.Value;
            this._logger = logger;
        }

        /// <summary>
        /// Gets the complete query with the keyword part and the filters part.
        /// </summary>
        /// <remarks>This is used by both the Query and Aggregation services.</remarks>
        /// <returns>A QueryContainer representing the entire query.  </returns>
        /// <param name="keyword">Keyword for the search</param>
        /// <param name="fullTextFieldsList">The complete full text fields list</param>
        /// <param name="filtersList">The complete filters list</param>
        protected QueryContainer GetFullQuery(string keyword, Dictionary<string, string[]> filtersList) {
            QueryContainer query = null;

            // Get list of full text fields from options for query building
            R4RAPIOptions.FullTextFieldConfig[] fullTextFieldsList = null;
            try
            {
                fullTextFieldsList = this._apiOptions.AvailableFullTextFields.Select(f => f.Value).ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError("Could not fetch full text fields from configuration.");
                throw new Exception("Could not fetch full text fields from configuration.", ex);
            }

            QueryContainer keywordQuery = GetKeywordQuery(keyword, fullTextFieldsList);
            IEnumerable<QueryContainer> filtersQueries = GetAllFiltersForQuery(filtersList);

            if (keywordQuery != null && filtersQueries.Count() > 0) {
                query = new BoolQuery
                {
                    Filter = filtersQueries,
                    Must = new QueryContainer[] { keywordQuery }
                };
            } else if (keywordQuery != null) {
                query = keywordQuery;
            } else if (filtersQueries.Count() > 0) {
                query = new BoolQuery
                {
                    Filter = filtersQueries
                };
            } //Else there is no query.

            return query;
        }

        /// <summary>
        /// Gets a query object to be used for all filters. 
        /// </summary>
        /// <remarks>
        /// When more than one filter is used we must use a Bool query (Must) to wrap the
        /// TermQuery objects that represent the filters. When only one filter is used, 
        /// then we only need to return a single TermQuery.
        /// </remarks>
        /// <returns>All of the filters for this query.  This is something that can be used for the filter
        /// portion of a bool query.</returns>
        /// <param name="filtersList">A dictionary containing of all of the filters. 
        /// The key should be the name of the field to filter.
        /// The values are a list of all of the filters.
        /// </param>
        protected IEnumerable<QueryContainer> GetAllFiltersForQuery(Dictionary<string,string[]> filtersList) {

            //NOTE: This assumes there are not dependencies between fields. (e.g. toolType & toolSubtype)
            //Therefore we are not required to do any complicated nested queries. This will work if all
            //the keys of the filters are unique. 
            //e.g. toolType: foo|toolSubtype: bar && toolType: bazz| toolSubtype: bar would not work.
            IEnumerable<QueryContainer> queries = new QueryContainer[]{};

            if (filtersList.Count == 1) {
                KeyValuePair<string, string[]> filter = filtersList.First();
                queries = new QueryContainer[] { GetQueryForFilterField($"{filter.Key}.key", filter.Value) };
            } else if (filtersList.Count > 1) {
                queries = from filter in filtersList
                          select GetQueryForFilterField($"{filter.Key}.key", filter.Value);
            }

            return queries;
        }

        /// <summary>
        /// Gets a query object used for filtering a field given one or more filters
        /// </summary>
        /// <remarks>
        /// When more than one filter is used we must use a Bool query (Should) to wrap the
        /// TermQuery objects that represent the filters. When only one filter is used, 
        /// then we only need to return a single TermQuery.
        /// </remarks>
        /// <returns>The QueryContainer to be used by the filter.</returns>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filters">The filters to turn into the query</param>
        /// <exception cref="ArgumentNullException">If there are 0 items in the filters list</exception>
        protected QueryContainer GetQueryForFilterField(string field, string[] filters) {
            QueryContainer query = null;

            if (filters.Length == 0)
            {
                throw new ArgumentException("Filters must contain at least one item");    
            }

            if (filters.Length == 1)
            {
                //There is only one, so it can just be a term query.
                query = GetQueryForField(field, filters[0]);
            }
            else
            {
                query = new BoolQuery { 
                    Should = from filter in filters
                                select (QueryContainer)GetQueryForField(field, filter),
                    MinimumShouldMatch = 1
                };
            }

            return query;
        }

        /// <summary>
        /// Gets a TermQuery for a given field.
        /// </summary>
        /// <returns>The query for field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="value">Value.</param>
        protected TermQuery GetQueryForField(string field, string value)
        {            
            TermQuery query = new TermQuery {
                Field = field,
                Value = value
            };

            return query;
        }

        /*
        /// <summary>
        /// Gets the list of full text fields to use for query building.
        /// </summary>
        /// <returns>The full text fields.</returns>
        protected FullTextField[] GetTextQueryDefinition()
        {
            var fields = new FullTextField[]
            {
                new FullTextField
                {
                    FieldName = "body._fulltext",
                    Boost = 1,
                    MatchTypes = new string[] { "common" }
                },
            };

            return fields;
        }*/

        /// <summary>
        /// Gets the keyword part of the query.
        /// </summary>
        /// <returns>The keyword query.</returns>
        /// <param name="keyword">Keyword.</param>
        /// <param name="fields">Full text fields.</param>
        protected QueryContainer GetKeywordQuery(string keyword, R4RAPIOptions.FullTextFieldConfig[] fields)
        {
            QueryContainer query = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = new BoolQuery
                {
                    Should = GetFullTextQuery(keyword, fields)
                };
            }
            return query;
        }

        /// <summary>
        /// Gets a list of QueryContainers for all fulltext fields.
        /// </summary>
        /// <returns>The QueryContainers for all fulltext fields.</returns>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="fields">Full-text fields.</param>
        protected IEnumerable<QueryContainer> GetFullTextQuery(string keyword, R4RAPIOptions.FullTextFieldConfig[] fields)
        {
            if(fields.Length > 0)
            {
                QueryContainer[] fullTextFieldQueries = fields.SelectMany(f => GetQueryForFullTextField(f.FieldName, keyword, f.Boost, f.MatchTypes)).ToArray();

                return fullTextFieldQueries;

                /*foreach(R4RAPIOptions.FullTextFieldConfig field in fields)
                {
                    foreach (QueryContainer q in GetQueryForFullTextField(field.FieldName, keyword, field.Boost, field.MatchTypes))
                        yield return q;
                }*/
            }
            else
            {
                throw new Exception("There must be more than one full text field for query.");
            }
        }
        
        /// <summary>
        /// Gets a QueryContainer for a given fulltext field.
        /// </summary>
        /// <returns>The query for the fulltext field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="boost">Boost.</param>
        /// <param name="matchTypes">Match types.</param>
        protected IEnumerable<QueryContainer> GetQueryForFullTextField(string field, string keyword, int boost, string[] matchTypes)
        {
            IEnumerable<QueryContainer> fullTextFieldQuery = new QueryContainer[] { };

            if (matchTypes.Length > 0)
            {
                fullTextFieldQuery = from matchType in matchTypes
                                     select GetQueryForMatchType(field, keyword, boost, matchType);
            }
            else
            {
                throw new ArgumentException($"Field {field} must have at least one match type.");
            }

            return fullTextFieldQuery;
        }

        /// <summary>
        /// Gets a QueryContainer for a given fulltext field's match type.
        /// </summary>
        /// <returns>The query for the specific match type of the fulltext field.</returns>
        /// <param name="field">Field.</param>
        /// <param name="keyword">Keyword text.</param>
        /// <param name="boost">Boost.</param>
        /// <param name="matchType">Match type.</param>
        protected QueryContainer GetQueryForMatchType(string field, string keyword, int boost, string matchType)
        {
            switch(matchType)
            {
                case "common":
                    return new CommonTermsQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost,
                        LowFrequencyOperator = Operator.And
                    };
                case "match":
                    return new MatchQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost
                    };
                case "match_phrase":
                    return new MatchPhraseQuery
                    {
                        Field = field,
                        Query = keyword,
                        Boost = boost
                    };
                default:
                    throw new ArgumentException($"Given match type {matchType} is for field {field} is invalid: must be common, match, or match_phrase.");
            }
        }
    }
}
