using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// The resources as returned by the IResourceQueryService
    /// </summary>
    public class ResourceQueryResult
    {
        /// <summary>
        /// The total number of results
        /// </summary>
        /// <value>The total results.</value>
        public int TotalResults { get; set; }

        /// <summary>
        /// The list of resources that matched the query
        /// </summary>
        /// <value>The results.</value>
        public Resource[] Results { get; set; } = new Resource[] { };
    }
}
