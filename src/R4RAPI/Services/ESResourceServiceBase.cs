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
        /// Gets a query object used for filtering a field given one or more filters
        /// </summary>
        /// <remarks>
        /// When more than one filter is used we must use a Bool query to wrap the
        /// TermQuery objects that represent the filters. When only one filter is used, 
        /// then we only need to return a single TermQuery.
        /// </remarks>
        /// <returns>The QueryContainer to be used by the filter.</returns>
        /// <param name="field">The field to filter on.</param>
        /// <param name="filters">The filters to turn into the query</param>
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
        protected TermQuery GetQueryForField(string field, string value, double boost = 0)
        {            
            TermQuery query = new TermQuery {
                Field = field,
                Value = value
            };

            if (boost > 0)
                query.Boost = boost;

            return query;
        }
    }
}
