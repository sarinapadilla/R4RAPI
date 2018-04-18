using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    public class ResourceAccess
    {
        [Keyword(Name = "type")]
        public string Type { get; set; }

        [Keyword(Name = "notes")]
        public string Notes { get; set; }
    }
}
