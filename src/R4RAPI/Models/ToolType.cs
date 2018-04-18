using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    public class ToolType
    {
        [Nested(Name = "type")]
        public KeyLabel Type { get; set; }

        [Nested(Name = "subtype")]
        public KeyLabel SubType { get; set; }
    }
}
