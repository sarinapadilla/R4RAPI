using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    public class ToolSubtype : KeyLabel
    {
        [Nested(Name = "parentKey")]
        public string ParentKey { get; set; }
    }
}
