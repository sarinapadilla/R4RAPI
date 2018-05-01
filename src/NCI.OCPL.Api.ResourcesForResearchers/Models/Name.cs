using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace NCI.OCPL.Api.ResourcesForResearchers.Models
{
    /// <summary>
    /// Contains the information about a name of a point of contact
    /// </summary>
    public class Name
    {
        /// <summary>
        /// The name prefix of the contact
        /// </summary>
        /// <value>The prefix.</value>
        [Keyword(Name = "prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// The first name of the contact
        /// </summary>
        /// <value>The first name.</value>
        [Keyword(Name = "firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// The middle name of the contact
        /// </summary>
        /// <value>The middle name.</value>
        [Keyword(Name = "middlename")]
        public string MiddleName { get; set; }

        /// <summary>
        /// The last name of the contact
        /// </summary>
        /// <value>The last name.</value>
        [Keyword(Name = "lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// The name suffix of the contact
        /// </summary>
        /// <value>The suffix.</value>
        [Keyword(Name = "suffix")]
        public string Suffix { get; set; }
    }
}
