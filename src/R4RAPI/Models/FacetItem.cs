using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    /// <summary>
    /// Describes the information about a facet item
    /// </summary>
    public class FacetItem
    {
        /// <summary>
        /// The key of the facet item
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// The label of the facet item
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// The count of the facet item (number of items categorized as this facet item)
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Whether or not this facet item is selected
        /// </summary>
        /// <value>A boolean reflecting whether this item is selected.</value>
        public bool Selected { get; set; }
    }
}
