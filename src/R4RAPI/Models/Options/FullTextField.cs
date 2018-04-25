using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    /// <summary>
    /// Represents a Full Text Field. This is used to build ES queries.
    /// </summary>
    public class FullTextField
    {
        /// <summary>
        /// Gets or sets the full text field name
        /// </summary>
        /// <value>The field name.</value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the boost value for the full text field
        /// </summary>
        /// <value>The boost value.</value>
        public int Boost { get; set; }

        /// <summary>
        /// Gets or sets the match types used in the query for this full text field
        /// </summary>
        /// <value>The match types.</value>
        public string[] MatchTypes { get; set; } = new string[] { };
    }
}
