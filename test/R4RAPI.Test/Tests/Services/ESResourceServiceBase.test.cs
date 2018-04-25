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
using System.Linq;

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
            public IEnumerable<QueryContainer> TEST_GetFullTextQuery(string keyword, FullTextField[] fields) => this.GetFullTextQuery(keyword, fields);
            public IEnumerable<QueryContainer> TEST_GetQueryForFullTextField(string field, string keyword, int boost, string[] matchTypes) => this.GetQueryForFullTextField(field, keyword, boost, matchTypes);
            public QueryContainer TEST_GetQueryForMatchType(string field, string keyword, int boost, string matchType) => this.GetQueryForMatchType(field, keyword, boost, matchType);
            
        }

        #region GetFullQuery

        #endregion

        #region GetAllFiltersForQuery

        public static IEnumerable<object[]> GetAllFiltersScenarioData => new[] {
            //Single Group, Single Param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG1", new string[]{"filter1"}} 
                },
                new string[]{
                    @"{ ""term"": { ""filterG1"": { ""value"": ""filter1"" } } }"
                }
            },
            //Two groups each with single param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG1", new string[]{"filter1"}},
                    {"filterG2", new string[]{"filter1"}}
                },
                new string[]{
                    @"{ ""term"": { ""filterG1"": { ""value"": ""filter1"" } } }",
                    @"{ ""term"": { ""filterG2"": { ""value"": ""filter1"" } } }"
                }
            },
            //Single Group, Multi Param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG1", new string[]{"filter1", "filter2"}}
                },
                new string[]{
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1"": { ""value"": ""filter2"" } } }
                            ], 
                            ""minimum_should_match"": 1,
                            ""disable_coord"": null,
                            ""_name"": null,
                            ""boost"": null
                        }
                    }
                    "
                }
            },
            //Multi Group, One Single and One Multi Param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG2", new string[]{"filter1"}},
                    {"filterG1", new string[]{"filter1", "filter2"}}
                },
                new string[]{
                    @"{ ""term"": { ""filterG2"": { ""value"": ""filter1"" } } }",
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }
                    "
                }
            },
            //Multi Group, One Single and One Multi Param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG2", new string[]{"filter1", "filter2"}},
                    {"filterG1", new string[]{"filter1", "filter2"}}
                },
                new string[]{
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG2"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG2"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }",
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }
                    "
                }
            },
        };

        [Theory, MemberData(nameof(GetAllFiltersScenarioData))]
        public void GetAllFiltersForQuery_Scenarios(Dictionary<string, string[]> filters, string[] expectedFilters)
        {
            var filterQueries = this._junkSvc.TEST_GetAllFiltersForQuery(filters);
            BoolQuery actual = new BoolQuery
            {
                Filter = filterQueries
            };

            string pre = @"{ ""bool"": { ""filter"": [";
            string post = @"], ""minimum_should_match"": null, ""disable_coord"": null,""_name"": null,""boost"": null}}";
            string expected = pre + string.Join(',', expectedFilters) + post;

            ElasticTools.AssertQueryJson(expected, actual);
        }

        [Fact]
        public void GetAllFiltersForQuery_GetEmpty()
        {
            var actual = this._junkSvc.TEST_GetAllFiltersForQuery(new Dictionary<string, string[]>());
            Assert.Equal(0, actual.Count());
        }

        #endregion

        #region GetQueryForFilterField 

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

        #endregion

        #region GetQueryForField

        [Fact]
        public void GetQueryForField()
        {

            var query = this._junkSvc.TEST_GetQueryForField("testfield", "testval");
            var expectedStr = @"
                { ""term"": { ""testfield"": { ""value"": ""testval"" } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        #endregion

        #region GetFullTextQuery
        [Fact]
        public void GetFullTextQuery_TwoFields()
        {
            var fullTextQuery = this._junkSvc.TEST_GetFullTextQuery(
                "testkeyword",
                new FullTextField[]
                {
                    new FullTextField
                    {
                        FieldName = "testfield1",
                        Boost = 1,
                        MatchTypes = new string[] { "common", "match" }
                    },
                    new FullTextField
                    {
                        FieldName = "testfield2",
                        Boost = 1,
                        MatchTypes = new string[] { "common" }
                    }
                });

            BoolQuery actual = new BoolQuery
            {
                Should = fullTextQuery
            };

            var expectedStr = @" 
                {
                    ""bool"": {
                        ""should"": [
                            { ""common"": { ""testfield1"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                            { ""match"": { ""testfield1"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                            { ""common"": { ""testfield2"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
                        ],
                        ""minimum_should_match"": null,
                        ""disable_coord"": null,
                        ""_name"": null,
                        ""boost"": null
                    }
                }
            ";

            ElasticTools.AssertQueryJson(expectedStr, actual);
        }

        #endregion

        #region GetQueryForFullTextField

        public static IEnumerable<object[]> GetQueryForFullTextFieldScenarioData => new[] {
            //Single Field, Single MatchType
            new object[] {
                "testkeyword",
                new FullTextField
                {
                    FieldName = "testfield",
                    Boost = 1,
                    MatchTypes = new string [] { "common" }
                },
                new string[]{
                    @"{ ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }"
                }
            },
            //Single Field, Multiple MatchTypes
            new object[] {
                "testkeyword",
                new FullTextField
                {
                    FieldName = "testfield",
                    Boost = 1,
                    MatchTypes = new string [] { "common", "match" }
                },
                new string[]{
                    @"
                        { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } }
                    "
                }
            }
        };

        [Theory, MemberData(nameof(GetQueryForFullTextFieldScenarioData))]
        public void GetQueryForFullTextField_Scenarios(string keyword, FullTextField field, string[] expectedFullTextFieldQueries)
        {
            var fullTextFieldQueries = this._junkSvc.TEST_GetQueryForFullTextField(field.FieldName, keyword, field.Boost, field.MatchTypes);
            BoolQuery actual = new BoolQuery
            {
                Should = fullTextFieldQueries
            };

            string pre = @"{ ""bool"": { ""should"": [";
            string post = @"], ""minimum_should_match"": null, ""disable_coord"": null,""_name"": null,""boost"": null } }";
            string expected = pre + string.Join(',', expectedFullTextFieldQueries) + post;

            ElasticTools.AssertQueryJson(expected, actual);
        }


        [Fact]
        public void GetQueryForFullTextField_MultipleMatchTypes()
        {
            var fullTextFieldQueries = this._junkSvc.TEST_GetQueryForFullTextField("testfield", "testkeyword", 1, new string[] { "common", "match_phrase" });
            BoolQuery actual = new BoolQuery
            {
                Should = fullTextFieldQueries
            };

            var expectedStr = @" 
                {
                    ""bool"": {
                        ""should"": [
                            { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                            { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } }
                        ],
                        ""minimum_should_match"": null,
                        ""disable_coord"": null,
                        ""_name"": null,
                        ""boost"": null
                    }
                }
            ";

            ElasticTools.AssertQueryJson(expectedStr, actual);
        }

        [Fact]
        public void GetQueryForFullTextField_SingleMatchType()
        {
            var fullTextFieldQueries = this._junkSvc.TEST_GetQueryForFullTextField("testfield", "testkeyword", 1, new string[] { "common" });
            BoolQuery actual = new BoolQuery
            {
                Should = fullTextFieldQueries
            };

            var expectedStr = @" 
                {
                    ""bool"": {
                        ""should"": [
                            { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
                        ],
                        ""minimum_should_match"": null,
                        ""disable_coord"": null,
                        ""_name"": null,
                        ""boost"": null
                    }
                }
            ";

            ElasticTools.AssertQueryJson(expectedStr, actual);
        }

        #endregion

        #region GetQueryForMatchType

        [Fact]
        public void GetQueryForMatchType_Common()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "common");
            var expectedStr = @" 
                { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        [Fact]
        public void GetQueryForMatchType_Match()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "match");
            var expectedStr = @" 
                { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        [Fact]
        public void GetQueryForMatchType_MatchPhrase()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "match_phrase");
            var expectedStr = @" 
                { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        [Fact]
        public void GetQueryForMatchType_GetInvalidMatchType()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "invalid");
            });
        }

        [Fact]
        public void GetQueryForMatchType_GetEmpty()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "");
            });
        }

        #endregion
    }
}
