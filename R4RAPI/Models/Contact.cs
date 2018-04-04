using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class Contact
    {
        public Name Name { get; set; }

        public string Title { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
