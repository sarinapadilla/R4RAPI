using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    public class Name
    {
        [Keyword(Name = "prefix")]
        public string Prefix { get; set; }

        [Keyword(Name = "firstname")]
        public string FirstName { get; set; }

        [Keyword(Name = "middlename")]
        public string MiddleName { get; set; }

        [Keyword(Name = "lastname")]
        public string LastName { get; set; }

        [Keyword(Name = "suffix")]
        public string Suffix { get; set; }
    }
}
