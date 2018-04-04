using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R4RAPI.Models
{
    public class Resource
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Website { get; set; }

        public string Description { get; set; }

        public ToolType[] ToolTypes { get; set; }

        public KeyLabel[] ResearchAreas { get; set; }

        public KeyLabel[] ResearchTypes { get; set; }

        public ResourceAccess ResourceAccess { get; set; }

        public string[] DOCs { get; set; }

        public Contact[] POCs { get; set; }

        public Resource(int id)
        {
            ID = id;
        }
    }
}
