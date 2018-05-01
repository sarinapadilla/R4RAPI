using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes the information about the returned results
    /// </summary>
    public class ResourceResults
    {
        /// <summary>
        /// The page metadata about the returned results
        /// </summary>
        /// <value>The page metadata.</value>
        public PageMetaData Meta { get; set; }

        /// <summary>
        /// a list of resources returned that match the query
        /// </summary>
        /// <value>The resources</value>
        public Resource[] Results { get; set; }

        /// <summary>
        /// A list of facets returned that match the query
        /// </summary>
        /// <value>The facets.</value>
        public Facet[] Facets { get; set; }
    }
}
