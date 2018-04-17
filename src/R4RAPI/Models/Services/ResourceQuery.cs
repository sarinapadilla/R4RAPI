using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class ResourceQuery
    {
        public string KeywordFilter { get; set; }

        public string ToolTypeFilter { get; set; }

        public string[] SubTypeFilter { get; set; }

        public string[] ResearchAreaFilter { get; set; }

        public string[] ResearchTypeFilter { get; set; }
    }
}
