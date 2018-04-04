using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class ResourceResults
    {
        public PageMetaData Meta { get; set; }

        public Resource[] Results { get; set; }

        public Facet[] Facets { get; set; }
    }
}
