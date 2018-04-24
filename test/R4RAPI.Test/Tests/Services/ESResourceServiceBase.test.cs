using System;

using Xunit;

using R4RAPI.Models;
using R4RAPI.Services;

using Elasticsearch.Net;
using Nest;

using NCI.OCPL.Utils.Testing;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace R4RAPI.Test.Services
{
    public class ESResourceServiceBase_Tests: TestESResourceServiceBase
    {
        readonly ServiceBaseTester _junkSvc;

        public ESResourceServiceBase_Tests(){
            this._junkSvc = this.GetService<ServiceBaseTester>(new ElasticsearchInterceptingConnection());
        }

        public class ServiceBaseTester : ESResourceServiceBase
        {

            public ServiceBaseTester(IElasticClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger logger): base(client, apiOptionsAccessor, logger) {}

            //Wrapping protected methods for testing
            public QueryContainer TEST_GetFullQuery(string keyword, Dictionary<string, string[]> filtersList) => this.GetFullQuery(keyword, filtersList);
            public QueryContainer TEST_GetKeywordQuery(string keyword) => this.GetKeywordQuery(keyword);
            public IEnumerable<QueryContainer> TEST_GetAllFiltersForQuery(Dictionary<string, string[]> filtersList) => this.GetAllFiltersForQuery(filtersList);
            public QueryContainer TEST_GetQueryForFilterField(string field, string[] filters) => this.GetQueryForFilterField(field, filters);
            public TermQuery TEST_GetQueryForField(string field, string value) => this.GetQueryForField(field, value);
        }

        [Fact]
        public void GetQueryForFilterField_GetEmpty()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                this._junkSvc.TEST_GetQueryForFilterField("testfield", new string[] { });
            });
        }


        [Fact]
        public void GetQueryForFilterField_GetMulti() {
            var query = this._junkSvc.TEST_GetQueryForFilterField("testfield", new string[] { "testval", "testval2" });

            //NOTE: ES automatically adds disable_coord,_name, and boost.
            var expectedStr = @"
                {
                    ""bool"": {
                        ""should"": [
                            { ""term"": { ""testfield"": { ""value"": ""testval"" } } },
                            { ""term"": { ""testfield"": { ""value"": ""testval2"" } } }
                        ], 
                        ""minimum_should_match"": 1,
                        ""disable_coord"": null,
                        ""_name"": null,
                        ""boost"": null
                    }
                }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        [Fact]
        public void GetQueryForFilterField_GetSingle()
        {            

            var query = this._junkSvc.TEST_GetQueryForFilterField("testfield", new string[] { "testval" });
            var expectedStr = @"
                { ""term"": { ""testfield"": { ""value"": ""testval"" } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);

        }

        [Fact]
        public void GetQueryForField()
        {

            var query = this._junkSvc.TEST_GetQueryForField("testfield", "testval");
            var expectedStr = @"
                { ""term"": { ""testfield"": { ""value"": ""testval"" } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

    }
}
