using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    public interface IResourceQueryService
    {
        /// <summary>
        /// Gets a resource from the API via its ID.
        /// </summary>
        /// <param name="id">The ID of the resource</param>
        /// <returns>The resource</returns>
        ResourceQueryResult Get(string id);

        /// <summary>
        /// Calls the search endpoint (/resources) of the R4R API
        /// </summary>
        /// <param name="query">Query parameters (optional)</param>
        /// <param name="size">Number of results to return (optional)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="includeFields">Fields to include (optional)</param>
        /// <param name="excludeFields">Fields to exclude (optional)</param>        
        /// <returns>Resource query result</returns>
        ResourceQueryResult Search(
            ResourceQuery query,
            int size = 10,
            int from = 0,
            string[] includeFields = null,
            string[] excludeFields = null
            );
    }
}
