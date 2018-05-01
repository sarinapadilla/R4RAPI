using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// An object that models a query for the IResourceSqueryService
    /// </summary>
    public class ResourceQuery
    {
        /// <summary>
        /// The keywords to use for the full-text search portion
        /// </summary>
        /// <value>The keyword.</value>
        public string Keyword { get; set; }

        /// <summary>
        /// The filters (aka selected facet items) to narrows the resources list.
        /// </summary>
        /// <value>The filters.</value>
        public Dictionary<string, string[]> Filters { get; set; } = new Dictionary<string, string[]>();
    }
}
