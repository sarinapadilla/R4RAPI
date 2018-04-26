using System;
using System.IO;
using System.Threading.Tasks;

using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using R4RAPI.Models;

namespace R4RAPI.Test.Services
{
    /// <summary>
    /// Class used for mocking BestBet Match requests to Elasticsearch.  This should be
    /// used as the base class of test specific Connections object passed into an ElasticClient. 
    /// </summary>
    /// <seealso cref="NCI.OCPL.Utils.Testing.ElasticsearchInterceptingConnection" />
    public class ESResQSvcGetConn : ElasticsearchInterceptingConnection
    {

        /// <summary>
        /// Gets the prefix of a testdata file for this test.
        /// </summary>
        /// <returns></returns>
        private string TestFile { get; set; }

        /// <summary>
        /// Creates a new instance of the ESResAggSvcConnection class
        /// </summary>
        /// <param name="testFile">The JSON file for the test response</param>
        public ESResQSvcGetConn(string testFile, int status = 200)
        {
            this.TestFile = testFile;

            //This section is for registering the intercepters for the request.

            //Add Handlers            
            this.RegisterRequestHandlerForType<Nest.GetResponse<Resource>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream(GetTestFileName());

                res.StatusCode = status;
            });
        }

        private string GetTestFileName()
        {
            return $"ESResQuerySvcData/{TestFile}.json";
        }
    }
}