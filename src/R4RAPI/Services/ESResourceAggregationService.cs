using System;
using Microsoft.Extensions.Logging;
using Nest;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    /// <summary>
    /// Service for fetching R4R Resource Aggregations
    /// </summary>
    public class ESResourceAggregationService : IResourceAggregationService
    {

        private IElasticClient _elasticClient;
        private readonly ILogger<ESResourceAggregationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceAggregationService"/> class.
        /// </summary>
        /// <param name="client">A configured Elasticsearch client</param>
        /// <param name="logger">A logger for logging.</param>
        public ESResourceAggregationService(IElasticClient client, ILogger<ESResourceAggregationService> logger)
        {
            this._elasticClient = client;
            this._logger = logger;
        }

        /// <summary>
        /// Gets the key label aggregation for a field
        /// </summary>
        /// <param name="field">The field to aggregate</param>
        /// <param name="query">The query for the results</param>
        /// <returns>The aggregation items</returns>
        public KeyLabelAggResult[] GetKeyLabelAggregation(string field, ResourceQuery query)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            if (query == null)
                throw new ArgumentNullException(nameof(query));

            //TODO: Config the index
            Indices index = Indices.Index(new string[] {"r4r_v1"});
            Types types = Types.Type(new string[] { "resource" });
            SearchRequest req = new SearchRequest(index, types)
            {
                Size = 0, //req.Size = 0; //Set the size to 0 in order to return no Resources

                // Let's check if the field has a period if it does, then we need
                // to handle it differently because it is either toolType.type or
                // toolType.subtype.
                Aggregations = field.Contains(".") ? GetComplexAgg(field, query) : GetSimpleAgg(field)
            };

            //We must(?) pass the C# type to map the results to
            //even though our queries should not return anything.
            var res = this._elasticClient.Search<Resource>(req);

            return new KeyLabelAggResult[] { };
        }

        /// <summary>
        /// Gets the simple aggregation "query"
        /// </summary>
        /// <returns>An AggregationDictionary containing the "query"</returns>
        /// <param name="field">Field to aggregate</param>
        private AggregationDictionary GetSimpleAgg(string field)
        {
            //Start with a nested aggregation
            var agg = new NestedAggregation($"{field}_agg")
            {
                Path = new Field(field), //Set the path of the nested agg
                                         // Add the sub aggregates (bucket keys)
                Aggregations = new TermsAggregation($"{field}_key")
                {
                    Field = new Field($"{field}.key"), //Set the field to rollup
                    Size = 999, //Use a large number to indicate unlimted (def is 10)
                                // Now, we need to get the labels for the keys and thus
                                // we need to add a sub aggregate for this term.  
                                // Normally you would do this for something like city/state rollups
                    Aggregations = new TermsAggregation($"{field}_label")
                    {
                        Field = new Field($"{field}.label")
                    }
                }
            };

            return agg;
        }

        /// <summary>
        /// Gets a complex (e.g. tooltype) simple aggregation "query"
        /// </summary>
        /// <returns>An AggregationDictionary containing the "query"</returns>
        /// <param name="field">Field to aggregate</param>
        /// <param name="query">The query object.</param>
        private AggregationDictionary GetComplexAgg(string field, ResourceQuery query)
        {
            // this is for things like tooltype.

            //Start with a nested aggregation
            var agg = new NestedAggregation($"{field}_agg")
            {
                Path = new Field(field), //Set the path of the nested agg
                                         // Add the sub aggregates (bucket keys)
                Aggregations = new TermsAggregation($"{field}_key")
                {
                    Field = new Field($"{field}.key"), //Set the field to rollup
                    Size = 999, //Use a large number to indicate unlimted (def is 10)
                                // Now, we need to get the labels for the keys and thus
                                // we need to add a sub aggregate for this term.  
                                // Normally you would do this for something like city/state rollups
                    Aggregations = new TermsAggregation($"{field}_label")
                    {
                        Field = new Field($"{field}.label")
                    }
                }
            };

            return agg;
        }

        /// <summary>
        /// Gets the Elasticsearch query for our Resource query.
        /// </summary>
        /// <param name="query">The query to get ES query parameters</param>
        protected void GetESQueryForQuery(ResourceQuery query) {
            
        }


    }
}
