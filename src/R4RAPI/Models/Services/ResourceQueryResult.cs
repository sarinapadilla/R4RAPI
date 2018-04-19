using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class ResourceQueryResult
    {
        public int TotalResults { get; set; }

        public Resource[] Results { get; set; }
    }
}
