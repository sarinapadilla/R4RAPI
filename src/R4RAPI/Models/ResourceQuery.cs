using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class ResourceQuery
    {
        public string Keyword { get; set; }

        public ToolType[] ToolTypeFilter { get; set; }

        public KeyLabel[] ResearchAreaFilter { get; set; }

        public KeyLabel[] ResearchTypeFilter { get; set; }
    }
}
