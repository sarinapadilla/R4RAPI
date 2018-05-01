using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Describes the information about a facet
    /// </summary>
    public class Facet
    {
        /// <summary>
        /// The title of the facet
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// The param name for the facet
        /// </summary>
        /// <value>The param.</value>
        public string Param { get; set; }

        /// <summary>
        /// A list of facet items within this facet
        /// </summary>
        /// <value>The facet items.</value>
        public FacetItem[] Items { get; set; } = new FacetItem[] { };
    }
}
