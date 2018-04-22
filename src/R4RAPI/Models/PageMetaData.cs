using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    /// <summary>
    /// Describes the page metadata for returned resource results
    /// </summary>
    public class PageMetaData
    {
        /// <summary>
        /// The total number of resources returned
        /// </summary>
        /// <value>The total number of results.</value>
        public int TotalResults { get; set; }

        /// <summary>
        /// The original query, for which the resource results are returned
        /// </summary>
        /// <value>The original query.</value>
        public string OriginalQuery { get; set; }
    }
}
