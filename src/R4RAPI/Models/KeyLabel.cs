using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    /// <summary>
    /// Describes an item's key and label
    /// </summary>
    public class KeyLabel
    {
        /// <summary>
        /// The key of the item
        /// </summary>
        /// <value>The key.</value>
        [Text(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// The label of the item
        /// </summary>
        /// <value>The label.</value>
        [Text(Name = "label")]
        public string Label { get; set; }
    }
}
