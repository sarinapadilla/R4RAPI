using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace R4RAPI.Models
{
    [ElasticsearchType(Name = "resource")]
    public class Resource
    {
        [Number(NumberType.Integer, Name = "id")]
        public int ID { get; set; }

        [Keyword(Name = "body")]
        public string Body { get; set; }

        [Keyword(Name = "title")]
        public string Title { get; set; }

        [Keyword(Name = "website")]
        public string Website { get; set; }

        [Keyword(Name = "description")]
        public string Description { get; set; }

        [Nested(Name = "toolTypes")]
        public KeyLabel[] ToolTypes { get; set; }

        [Nested(Name = "toolSubtypes")]
        public ToolSubtype[] ToolSubtypes { get; set; }

        [Nested(Name = "researchAreas")]
        public KeyLabel[] ResearchAreas { get; set; }

        [Nested(Name = "researchTypes")]
        public KeyLabel[] ResearchTypes { get; set; }

        [Nested(Name = "resourceAccess")]
        public ResourceAccess ResourceAccess { get; set; }

        [Nested(Name = "docs")]
        public KeyLabel[] DOCs { get; set; }

        [Nested(Name = "pocs")]
        public Contact[] POCs { get; set; }
    }
}
