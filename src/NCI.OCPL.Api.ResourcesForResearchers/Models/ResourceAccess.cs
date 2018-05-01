using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes the information about the resource access of a resource
    /// </summary>
    public class ResourceAccess
    {
        /// <summary>
        /// The type of resource access
        /// </summary>
        /// <value>The type.</value>
        [Keyword(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// The notes about the resource access
        /// </summary>
        /// <value>The notes.</value>
        [Keyword(Name = "notes")]
        public string Notes { get; set; }
    }
}
