using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class FacetIem
    {
        public string Key { get; set; }

        public string Label { get; set; }

        public int Count { get; set; }

        public bool Selected { get; set; }
    }
}
