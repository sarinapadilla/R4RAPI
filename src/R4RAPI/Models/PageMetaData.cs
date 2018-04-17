using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class PageMetaData
    {
        public int TotalResults { get; set; }

        public string OriginalQuery { get; set; }
    }
}
