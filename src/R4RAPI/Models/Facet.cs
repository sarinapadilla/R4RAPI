using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class Facet
    {
        public string Title { get; set; }

        public string Param { get; set; }

        public FacetItem[] Items { get; set; } = new FacetItem[] { };
    }
}
