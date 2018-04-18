using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    public class KeyLabel
    {
        [Text(Name = "key")]
        public string Key { get; set; }

        [Text(Name = "label")]
        public string Label { get; set; }
    }
}
