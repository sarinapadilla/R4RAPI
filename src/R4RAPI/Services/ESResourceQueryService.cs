using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    /// <summary>
    /// Service for fetching R4R Resources
    /// </summary>
    public class ESResourceQueryService : IResourceQueryService
    {
        private IElasticClient _elasticClient;
        private R4RAPIOptions _apiOptions;
        private readonly ILogger<ESResourceQueryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Services.ESResourceQueryService"/> class.
        /// </summary>
        /// <param name="client">A configured Elasticsearch client</param>
        /// <param name="logger">A logger for logging.</param>
        public ESResourceQueryService(IElasticClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger<ESResourceQueryService> logger)
        {
            this._elasticClient = client;
            this._apiOptions = apiOptionsAccessor.Value;
            this._logger = logger;
        }

        /// <summary>
        /// Gets a resource from the API via its ID.
        /// </summary>
        /// <param name="id">The ID of the resource</param>
        /// <returns>The resource</returns>
        public Resource Get(string id)
        {
            Resource resResult = null;

            // If the given ID is null or empty, throw an exception
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("The resource identifier is null or an empty string.");
            }

            // Validate if given ID is correctly formatted as an int
            int resID;
            bool validID = int.TryParse(id, out resID);

            if(validID)
            {
                var response = _elasticClient.Get<Resource>(new GetRequest(this._apiOptions.AliasName, "resource", resID));
                if(response.Found && response.IsValid)
                {
                    resResult = response.Source;
                }
                else if (response.Found && !response.IsValid)
                {
                    this._logger.LogError("Bad request.");
                }
                else if (!response.Found && response.IsValid)
                {
                    this._logger.LogError("Resource not found.");
                }
            }

            return resResult;
        }

        /// <summary>
        /// Gets the resources that match the given params
        /// </summary>
        /// <param name="query">Query parameters (optional)</param>
        /// <param name="size">Number of results to return (optional)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="includeFields">Fields to include (optional)</param>       
        /// <returns>Resource query result</returns>
        public ResourceQueryResult QueryResources(
            ResourceQuery query,
            int size = 20,
            int from = 0,
            string[] includeFields = null
            )
        {
            ResourceQueryResult queryResults = new ResourceQueryResult();

            Indices index = Indices.Index(new string[] { "r4r_v1" });
            Types types = Types.Type(new string[] { "resource" });
            SearchRequest request = new SearchRequest(index, types)
            {
                Size = size,
                From = from, 
            };

            var response = this._elasticClient.Search<Resource>(request);

            List<Resource> resourceResults = new List<Resource>();

            if (!response.IsValid)
            {
                this._logger.LogError("Bad request.");
            }

            if (response.Total > 0)
            {
                foreach (Resource res in response.Documents)
                {
                    resourceResults.Add(res);
                    queryResults.Results = resourceResults.ToArray();
                    queryResults.TotalResults = Convert.ToInt32(response.Total);
                }
            }

            return queryResults;
        }
    }
}
