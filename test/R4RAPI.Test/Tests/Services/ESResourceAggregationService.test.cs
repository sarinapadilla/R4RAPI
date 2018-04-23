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
    public class ESResourceAggregationServiceTests : TestESResourceServiceBase
    {

        #region Test Query Building

        [Fact]
        public void GetKLA_Build_SubType_Missing_Tooltype() {
            
        }

        [Fact]
        public void GetKLA_Build_SubType()
        {
            //Create new ESRegAggConnection...

            string actualPath = "";
            string expectedPath = "r4r_v1/resource/_search"; //Use index in config

            JObject actualRequest = null;
            JObject expectedRequest = JObject.Parse(@"
                {
                    ""size"": 0,
                    ""aggs"": {
                        ""toolSubtypes_agg"": {
                            ""nested"": {
                                ""path"": ""toolSubtypes""
                            },
                            ""aggs"": {
                                ""toolSubtypes_filter"": {
                                    ""filter"": {
                                        ""term"": { ""toolSubtypes.parentKey"": { ""value"": ""datasets_databases"" } }                                        
                                    },
                                    ""aggs"": {
                                        ""toolSubtypes_key"": {
                                            ""terms"": {
                                                ""field"": ""toolSubtypes.key"",
                                                ""size"": 999
                                            },
                                            ""aggs"": {
                                                ""toolSubtypes_label"": {
                                                    ""terms"": {
                                                        ""field"": ""toolSubtypes.label""
                                                    }
                                                }
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

            ESResourceAggregationService aggSvc = this.GetService<ESResourceAggregationService>(conn);
            try
            {
                KeyLabelAggResult[] aggResults = aggSvc.GetKeyLabelAggregation(
                    "toolSubtypes", 
                    new ResourceQuery {
                        Filters = new Dictionary<string,string[]> {
                        { "toolTypes", new string[] { "datasets_databases" } }
                        } 
                    }
                );
            }
            catch (Exception ex) {
                int i = 1;
            } //We don't care how it processes the results...


            Assert.Equal(expectedPath, actualPath);
            Assert.Equal(expectedRequest, actualRequest);
        }

        [Fact]
        public void GetKeyLabelAggregation_Build_EmptyQuery() {
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

            ESResourceAggregationService aggSvc = this.GetService<ESResourceAggregationService>(conn);
            try
            {
                KeyLabelAggResult[] aggResults = aggSvc.GetKeyLabelAggregation("researchTypes", new ResourceQuery());
            } catch (Exception) {} //We don't care how it processes the results...


            Assert.Equal(expectedPath, actualPath);
            Assert.Equal(expectedRequest, actualRequest);
        }
        #endregion

        #region Test Results Parsing
        /// <summary>
        /// Tests non-nested aggregation without query
        /// THIS TEST IS FOR MAKING SURE THE RESULTS ARE MAPPED CORRECTLY
        /// </summary>
        [Fact]
        public void GetKeyLabelAggregation_EmptyQuery() {
            //Create new ESRegAggConnection...

            IConnection conn = new ESResAggSvcConnection("ResearchTypes_EmptyQuery");

            //Expected Aggs
            KeyLabelAggResult[] expectedAggs = new KeyLabelAggResult[] {
                new KeyLabelAggResult() {
                    Key = "basic",
                    Label = "Basic",
                    Count = 94
                },
                new KeyLabelAggResult() {
                    Key = "translational",
                    Label = "Translational",
                    Count = 66
                },
                new KeyLabelAggResult() {
                    Key = "clinical_trials",
                    Label = "Clinical Trials",
                    Count = 42
                },
                new KeyLabelAggResult() {
                    Key = "epidemiologic",
                    Label = "Epidemiologic",
                    Count = 26
                },
                new KeyLabelAggResult() {
                    Key = "clinical",
                    Label = "Clinical",
                    Count = 5
                }
            };

            ESResourceAggregationService aggSvc = this.GetService<ESResourceAggregationService>(conn);
            KeyLabelAggResult[] actualAggs = aggSvc.GetKeyLabelAggregation("researchTypes", new ResourceQuery());

            //Order does matter here, so we can compare the arrays
            Assert.Equal(expectedAggs, actualAggs, new KeyLabelAggResultComparer());

        }


        [Fact]
        public void GetKLA_SubType()
        {
            //Create new ESRegAggConnection...

            IConnection conn = new ESResAggSvcConnection("SubtoolTypes_WithToolType");

            //Expected Aggs
            KeyLabelAggResult[] expectedAggs = new KeyLabelAggResult[] {
                new KeyLabelAggResult() {
                    Key = "clinical_data",
                    Label = "Clinical Data",
                    Count = 21
                },
                new KeyLabelAggResult() {
                    Key = "imaging",
                    Label = "Imaging",
                    Count = 14
                },
                new KeyLabelAggResult() {
                    Key = "genomic_datasets",
                    Label = "Genomic Datasets",
                    Count = 13
                },
                new KeyLabelAggResult() {
                    Key = "epidemiologic_data",
                    Label = "Epidemiologic Data",
                    Count = 12
                },
                new KeyLabelAggResult() {
                    Key = "patient_registries",
                    Label = "Patient Registries",
                    Count = 3
                },
                new KeyLabelAggResult() {
                    Key = "biological_networks",
                    Label = "Biological Networks",
                    Count = 2
                }
            };

            ESResourceAggregationService aggSvc = this.GetService<ESResourceAggregationService>(conn);
            KeyLabelAggResult[] actualAggs = aggSvc.GetKeyLabelAggregation(
                "toolSubtypes",
                new ResourceQuery
                {
                    Filters = new Dictionary<string, string[]> {
                                    { "toolTypes", new string[] { "datasets_databases" } }
                    }
                }
            );

            //Order does matter here, so we can compare the arrays
            Assert.Equal(expectedAggs, actualAggs, new KeyLabelAggResultComparer());

        }
        #endregion


    }
}
