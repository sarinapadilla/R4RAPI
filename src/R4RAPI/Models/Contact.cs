using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    /// <summary>
    /// Describes the information about a point of contact
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// The name of the contact
        /// </summary>
        /// <value>The name.</value>
        public Name Name { get; set; }

        /// <summary>
        /// The title of the contact
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// The phone number of the contact
        /// </summary>
        /// <value>The phone number.</value>
        public string Phone { get; set; }

        /// <summary>
        /// The email of the contact
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }
    }
}
