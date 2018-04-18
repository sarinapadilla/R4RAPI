using System;
using System.IO;
using System.Threading.Tasks;

using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace R4RAPI.Test.Services
{
    /// <summary>
    /// Class used for mocking BestBet Match requests to Elasticsearch.  This should be
    /// used as the base class of test specific Connections object passed into an ElasticClient. 
    /// </summary>
    /// <seealso cref="NCI.OCPL.Utils.Testing.ElasticsearchInterceptingConnection" />
    public class ESResAggSvcConnection : ElasticsearchInterceptingConnection
    {

        /// <summary>
        /// Gets the prefix of a testdata file for this test.
        /// </summary>
        /// <returns></returns>
        private string TestFilePrefix { get; set; }

        /// <summary>
        /// Creates a new instance of the ESResAggSvcConnection class
        /// </summary>
        /// <param name="testFilePrefix">The prefix of the test files</param>
        public ESResAggSvcConnection(string testFilePrefix)
        {
            this.TestFilePrefix = testFilePrefix;

            //This section is for registering the intercepters for the request.

            //Add Handlers            
            //this.RegisterRequestHandlerForType<Nest.SearchResponse<BestBetsMatch>>((req, res) =>
            //{
            //    //Get the request parameters
            //    dynamic postObj = this.GetRequestPost(req);

                //Determine which round we are performing
            //    int numTokens = postObj["params"].matchedtokencount;

                //Get the file name for this round
            //    res.Stream = TestingTools.GetTestFileAsStream(GetTestFileName(numTokens));

            //    res.StatusCode = 200;
            //});

        }

        private string GetTestFileName()
        {
            return $"ESResAggSvcData/{TestFilePrefix}.json";
        }
    }
}