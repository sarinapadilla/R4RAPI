using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    public interface IResourceAggregateService
    {
        /// <summary>
        /// Calls the search endpoint (/resources) of the R4R API
        /// </summary>
        /// <param name="">Query parameters (optional)</param> 
        /// <param name="query">Query parameters (optional)</param>      
        /// <returns>Resource query result</returns>
        ResourceQueryResult Aggregate(
            string facet,
            ResourceQuery query
            );
    }
}
