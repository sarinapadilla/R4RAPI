using System;

using Xunit;

using R4RAPI.Models;
using R4RAPI.Services;

using Elasticsearch.Net;
using Nest;

using NCI.OCPL.Utils.Testing;
using Microsoft.Extensions.Logging.Testing;
using Newtonsoft.Json.Linq;

namespace R4RAPI.Test.Services
{
    public class ESResourceAggregationServiceTests
    {

        #region Test Query Building
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

            ESResourceAggregationService aggSvc = GetAggService(conn);
            KeyLabelAggResult[] aggResults = aggSvc.GetKeyLabelAggregation("researchTypes", new ResourceQuery());


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
            KeyLabelAggResult[] expectedAggs = new KeyLabelAggResult[] { };

            ESResourceAggregationService aggSvc = GetAggService(conn);
            KeyLabelAggResult[] actualAggs = aggSvc.GetKeyLabelAggregation("researchTypes", new ResourceQuery());

            //Order does matter here, so we can compare the arrays
            Assert.Equal(expectedAggs, actualAggs, new KeyLabelAggResultComparer());

        }
        #endregion

        private ESResourceAggregationService GetAggService(IConnection connection)
        {
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);
            IElasticClient client = new ElasticClient(connectionSettings);

            //We don't need any options yet
            //IOptions<CGBBIndexOptions> config = GetMockConfig();

            return new ESResourceAggregationService(client, new NullLogger<ESResourceAggregationService>());
        }
    }
}
