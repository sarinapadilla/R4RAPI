using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Configuration options for the R4R API
    /// </summary>
    public class R4RAPIOptions
    {
        /// <summary>
        /// Gets or sets the alias name for the Elasticsearch Collection we will use
        /// </summary>
        /// <value>The name of the alias.</value>
        public string AliasName { get; set; }

        /// <summary>
        /// Gets or sets the available facets and their configuration for the API
        /// </summary>
        /// <value>The available facets.</value>
        public Dictionary<string, FacetConfig> AvailableFacets { get; set; } = new Dictionary<string, FacetConfig>();

        /// <summary>
        /// Gets or sets the available full text fields and their configuration for the API
        /// </summary>
        /// <value>The available full text fields.</value>
        public Dictionary<string,FullTextFieldConfig> AvailableFullTextFields { get; set; } = new Dictionary<string, FullTextFieldConfig>();

        #region Sub classes

        /// <summary>
        /// Defines a list of the facet fetch/display types.
        /// </summary>
        public enum FacetTypes
        {
            /// <summary>
            /// The facet should only allow for single selections
            /// </summary>
            Single,
            /// <summary>
            /// The facet allows for multiple selctions
            /// </summary>
            Multiple,

            //Additional facets could be
            //Heirarchical - the facet allows nesting
            //Range - the facet is a ranged facet 
            //etc.
        }

        /// <summary>
        /// Configuration information for a facet
        /// </summary>
        public class FacetConfig
        {
            /// <summary>
            /// Gets or sets the name of the filter parameter to be used to filter
            /// this facet.
            /// </summary>
            /// <value>The name of the filter.</value>
            public string FilterName { get; set; }

            /// <summary>
            /// Gets or sets the display label for this facet
            /// </summary>
            /// <value>The label.</value>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="T:R4RAPI.Models.R4RAPIOptions.FacetConfig"/>
            /// should be included when the includeFacets is empty.
            /// </summary>
            /// <value><c>true</c> if include in default; otherwise, <c>false</c>.</value>
            public bool IncludeInDefault { get; set; } = false;

            /// <summary>
            /// Gets the type of facet for this facet
            /// </summary>
            /// <value>The type of the facet.</value>
            public FacetTypes FacetType { get; set; }

            /// <summary>
            /// Gets or sets the name of the filter parameter this facet depends
            /// on for its parentKey.
            /// </summary>
            /// <value>The requires filter.</value>
            public string RequiresFilter { get; set; }

        }

        /// <summary>
        /// Configuration information for a full text field
        /// </summary>
        public class FullTextFieldConfig
        {
            /// <summary>
            /// Gets or sets the name of the full text field.
            /// </summary>
            /// <value>The name of the field.</value>
            public string FieldName { get; set; }

            /// <summary>
            /// Gets or sets the boost value for the full text field.
            /// </summary>
            /// <value>The boost value.</value>
            public int Boost { get; set; }

            /// <summary>
            /// Gets or sets the match types for the full text field.
            /// </summary>
            /// <value>The match types.</value>
            public string[] MatchTypes { get; set; } = new string[] { };
        }

        #endregion
    }
}
