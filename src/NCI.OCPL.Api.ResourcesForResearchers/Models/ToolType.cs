using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes the information about the tool type of an resource
    /// </summary>
    public class ToolSubtype : KeyLabel
    {
        /// <summary>
        /// The parent key of a tool subtype
        /// </summary>
        /// <value>The parent key.</value>
        [Nested(Name = "parentKey")]
        public string ParentKey { get; set; }
    }
}
