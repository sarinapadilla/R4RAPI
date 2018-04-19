using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Moq;

using Elasticsearch.Net;
using Nest;

using NCI.OCPL.Utils.Testing;
using R4RAPI.Models;

namespace R4RAPI.Test.Services
{
    /// <summary>
    /// Provides common methods used by service test classes.
    /// </summary>
    public class TestESResourceServiceBase
    {

        /// <summary>
        /// Gets an instance of a service that inherits from TestESResourceServiceBase
        /// </summary>
        /// <returns>The service.</returns>
        /// <param name="connection">Connection.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected T GetService<T>(IConnection connection) where T : class
        {
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);
            IElasticClient client = new ElasticClient(connectionSettings);

            //We don't need any options yet
            //IOptions<CGBBIndexOptions> config = GetMockConfig();

            return (T)Activator.CreateInstance(typeof(T), new object[] { client, GetMockConfig(), new NullLogger<T>() });
        }

        /// <summary>
        /// Helper method to create a mocked up CGBBIndexOptions object.
        /// </summary>
        /// <returns></returns>
        protected IOptions<R4RAPIOptions> GetMockConfig()
        {
            Moq.Mock<IOptions<R4RAPIOptions>> config = new Mock<IOptions<R4RAPIOptions>>();
            config
                .SetupGet(o => o.Value)
                .Returns(new R4RAPIOptions()
                {
                    AliasName = "r4r_v1"
                });

            return config.Object;
        }

    }
}
