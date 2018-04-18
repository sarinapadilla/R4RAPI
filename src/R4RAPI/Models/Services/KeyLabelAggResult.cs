using System;

namespace R4RAPI.Models
{
    /// <summary>
    /// Represents a Key/Label Aggregation. This is used when the filtering
    /// key is not as pretty as a display label.
    /// </summary>
    public class KeyLabelAggResult
    {
        /// <summary>
        /// Gets or sets the key for filtering of this agg result
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the label for display of this agg result
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the number of records that this facet would match.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

    }
}
