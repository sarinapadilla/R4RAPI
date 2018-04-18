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
                        ""researchType_agg"": {
                            ""nested"": {
                                ""path"": ""researchType""
                            },
                            ""aggs"": {
                                ""researchType_key"": {
                                    ""terms"": {
                                        ""field"": ""researchType.key"",
                                        ""size"": 999
                                    },
                                    ""aggs"": {
                                        ""researchType_label"": {
                                            ""terms"": {
                                                ""field"": ""researchType.label""
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
            KeyLabelAggResult[] aggResults = aggSvc.GetKeyLabelAggregation("researchType", new ResourceQuery());


            Assert.Equal(expectedPath, actualPath);
            Assert.Equal(expectedRequest, actualRequest);
        }
        #endregion

        #region Test Results Parsing
        /// <summary>
        /// Tests non-nested aggregation without query
        /// THIS TEST IS FOR MAKING SURE THE RESULTS ARE MAPPED CORRECTLY
        /// </summary>
        //[Fact]
        //public void GetKeyLabelAggregation_EmptyQuery() {
        //Create new ESRegAggConnection...

        //    IConnection conn = new ESResAggSvcConnection("blah");

        //    ESResourceAggregationService aggSvc = GetAggService(conn);

        //}
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
