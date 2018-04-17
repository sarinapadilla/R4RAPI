using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class ResourceQuery
    {
        public string Keyword { get; set; }

        public Dictionary<string, string[]> Filters { get; set; } = new Dictionary<string, string[]>();
    }
}
