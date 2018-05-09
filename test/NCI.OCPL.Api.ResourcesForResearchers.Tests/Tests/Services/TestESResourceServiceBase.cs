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
using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
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
            var options = new R4RAPIOptions
            {
                AliasName = "r4r_v1",
                AvailableFacets = new Dictionary<string, R4RAPIOptions.FacetConfig> {
                    {
                        "toolTypes", new R4RAPIOptions.FacetConfig {
                            FilterName = "toolTypes",
                            Label = "Tool Types",
                            FacetType = R4RAPIOptions.FacetTypes.Single,
                            IncludeInDefault = true
                        }
                    },
                    {
                        "toolSubtypes", new R4RAPIOptions.FacetConfig {
                            FilterName= "toolSubtypes",
                            Label= "Tool Sub-Types",
                            FacetType= R4RAPIOptions.FacetTypes.Multiple,
                            IncludeInDefault= true,
                            RequiresFilter= "toolTypes"
                        }
                    },
                    {
                        "researchAreas", new R4RAPIOptions.FacetConfig {
                            FilterName= "researchAreas",
                            Label= "Research Areas",
                            FacetType= R4RAPIOptions.FacetTypes.Multiple,
                            IncludeInDefault= true
                        }
                    },
                    {
                        "researchTypes", new R4RAPIOptions.FacetConfig {
                            FilterName= "researchTypes",
                            Label= "Research Types",
                            FacetType= R4RAPIOptions.FacetTypes.Multiple,
                            IncludeInDefault= true
                        }
                    },
                    {
                        "docs", new R4RAPIOptions.FacetConfig {
                            FilterName= "docs",
                            Label= "Division, Offices, and Centers",
                            FacetType= R4RAPIOptions.FacetTypes.Multiple,
                            IncludeInDefault= false
                        }
                    }
                },
                AvailableFullTextFields = new Dictionary<string, R4RAPIOptions.FullTextFieldConfig> {
                    {
                        "title", new R4RAPIOptions.FullTextFieldConfig {
                            FieldName = "title._fulltext",
                            Boost = 1,
                            MatchTypes = new string[] { "common", "match", "match_phrase" }
                        }
                    },
                    {
                        "body", new R4RAPIOptions.FullTextFieldConfig {
                            FieldName = "body._fulltext",
                            Boost = 1,
                            MatchTypes = new string[] { "common", "match", "match_phrase" }
                        }
                    },
                    {
                        "pocs.lastname", new R4RAPIOptions.FullTextFieldConfig {
                            FieldName = "pocs.lastname._fulltext",
                            Boost = 1,
                            MatchTypes = new string[] { "match"}
                        }
                    },
                    {
                        "pocs.firstname", new R4RAPIOptions.FullTextFieldConfig {
                            FieldName = "pocs.firstname._fulltext",
                            Boost = 1,
                            MatchTypes = new string[] { "match"}
                        }
                    },
                    {
                        "pocs.middlename", new R4RAPIOptions.FullTextFieldConfig {
                            FieldName = "pocs.middlename._fulltext",
                            Boost = 1,
                            MatchTypes = new string[] { "match"}
                        }
                    }
                }
            };

            Moq.Mock<IOptions<R4RAPIOptions>> config = new Mock<IOptions<R4RAPIOptions>>();
            config
                .SetupGet(o => o.Value)
                .Returns(options);

            return config.Object;
        }

    }
}
