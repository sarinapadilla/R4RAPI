using System;

using Xunit;

using R4RAPI.Models;
using R4RAPI.Services;

using Elasticsearch.Net;
using Nest;

using NCI.OCPL.Utils.Testing;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace R4RAPI.Test.Services
{
    public class ESResourceQueryService_Tests : TestESResourceServiceBase
    {
        #region Test Query Building

        [Fact]
        public void QueryResources_EmptyQuery() {
            
        }

        [Fact]
        public void QueryResources_SingleFilter()
        {
            //Create new ESRegAggConnection...

            string actualPath = "";
            string expectedPath = "r4r_v1/resource/_search"; //Use index in config

            JObject actualRequest = null;
            JObject expectedRequest = JObject.Parse(@"
                {
                    ""size"": 0,
                    ""aggs"": {
                        ""researchTypes_agg"": {
                            ""nested"": {
                                ""path"": ""researchTypes""
                            },
                            ""aggs"": {
                                ""researchTypes_key"": {
                                    ""terms"": {
                                        ""field"": ""researchTypes.key"",
                                        ""size"": 999
                                    },
                                    ""aggs"": {
                                        ""researchTypes_label"": {
                                            ""terms"": {
                                                ""field"": ""researchTypes.label""
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } 
            ");
            /*
            */

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            //SearchResponse<Resource> <-- type
            conn.RegisterRequestHandlerForType<SearchResponse<Resource>>((req, res) =>
            {
                actualPath = req.Path;
                actualRequest = conn.GetRequestPost(req);
            });

            var svc = this.GetService<ESResourceQueryService>(conn);
            try
            {
                var results = svc.QueryResources(
                    new ResourceQuery
                    {
                        Filters = new Dictionary<string, string[]>{
                            { "researchTypes", new string[] { "basic"} }
                        }
                    }
                );
            }
            catch (Exception) { } //We don't care how it processes the results...


            Assert.Equal(expectedPath, actualPath);
            Assert.Equal(expectedRequest, actualRequest);            
        }

        #endregion

    }
}
