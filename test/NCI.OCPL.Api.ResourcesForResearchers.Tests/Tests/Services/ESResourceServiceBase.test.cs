using System;

using Xunit;

using NCI.OCPL.Api.ResourcesForResearchers.Models;
using NCI.OCPL.Api.ResourcesForResearchers.Services;

using Elasticsearch.Net;
using Nest;

using NCI.OCPL.Utils.Testing;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
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
            public IEnumerable<QueryContainer> TEST_GetFullTextQuery(string keyword, R4RAPIOptions.FullTextFieldConfig[] fields) => this.GetFullTextQuery(keyword, fields);
            public IEnumerable<QueryContainer> TEST_GetQueryForFullTextField(string field, string keyword, int boost, string[] matchTypes) => this.GetQueryForFullTextField(field, keyword, boost, matchTypes);
            public QueryContainer TEST_GetQueryForMatchType(string field, string keyword, int boost, string matchType) => this.GetQueryForMatchType(field, keyword, boost, matchType);
            
        }

        #region GetFullQuery

        public static IEnumerable<object[]> GetFullQueryScenarioData => new[] {
            //Multi Group, Both Multi Param
            new object[] {
                "testkeyword",
                new string[]{
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""common"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                                { ""match"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                                { ""match"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } },
                                { ""common"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                                { ""match"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                                { ""match"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } },
                                { ""match"": { ""pocs.lastname._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                                { ""match"": { ""pocs.firstname._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                                { ""match"": { ""pocs.middlename._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } }
                            ], ""minimum_should_match"": null, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }"
                },
                new Dictionary<string, string[]>{
                    {"filterG2", new string[]{"filter1", "filter2"}},
                    {"filterG1", new string[]{"filter1", "filter2"}}
                },
                new string[]{
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG2.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG2.key"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }",
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }
                    "
                }
            }
        };

        [Theory, MemberData(nameof(GetFullQueryScenarioData))]
        public void GetFullQuery_Scenarios(string keyword, string[] expectedFullTextQuery, Dictionary<string, string[]> filters, string[] expectedFilters)
        {
            var actual = this._junkSvc.TEST_GetFullQuery(keyword, filters);

            string preKeyword = @"{ ""bool"": { ""must"": [ ";
            string postKeyword = @"], ";
            string preFilter = @"""filter"": [";
            string postFilter = @"], ""minimum_should_match"": null, ""disable_coord"": null,""_name"": null,""boost"": null } }";

            string expected = preKeyword + string.Join(',', expectedFullTextQuery) + postKeyword + preFilter + string.Join(',', expectedFilters) + postFilter;

            ElasticTools.AssertQueryJson(expected, actual);
        }

        #endregion

        #region GetAllFiltersForQuery

        public static IEnumerable<object[]> GetAllFiltersScenarioData => new[] {
            //Single Group, Single Param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG1", new string[]{"filter1"}} 
                },
                new string[]{
                    @"{ ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } }"
                }
            },
            //Two groups each with single param
            new object[] {
                new Dictionary<string, string[]>{
                    {"filterG1", new string[]{"filter1"}},
                    {"filterG2", new string[]{"filter1"}}
                },
                new string[]{
                    @"{ ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } }",
                    @"{ ""term"": { ""filterG2.key"": { ""value"": ""filter1"" } } }"
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
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter2"" } } }
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
                    @"{ ""term"": { ""filterG2.key"": { ""value"": ""filter1"" } } }",
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }
                    "
                }
            },
            //Multi Group, Both Multi Param
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
                                { ""term"": { ""filterG2.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG2.key"": { ""value"": ""filter2"" } } }
                            ], ""minimum_should_match"": 1, ""disable_coord"": null, ""_name"": null, ""boost"": null
                        }
                    }",
                    @"
                    {
                        ""bool"": {
                            ""should"": [
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter1"" } } },
                                { ""term"": { ""filterG1.key"": { ""value"": ""filter2"" } } }
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

        #region GetKeywordQuery
        
        public static IEnumerable<object[]> GetKeywordQueryScenarioData => new[] {
            // Multiple fields, multiple match types
            new object[] {
                "testkeyword",
                new string[]{
                    @"
                        { ""common"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""title._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } },
                        { ""common"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""body._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } },
                        { ""match"": { ""pocs.lastname._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""pocs.firstname._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""pocs.middlename._fulltext"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                    "
                }
            }
        };

        [Theory, MemberData(nameof(GetKeywordQueryScenarioData))]
        public void GetKeywordQuery_Scenarios(string keyword, string[] expectedFullTextQuery)
        {
            var actual = this._junkSvc.TEST_GetKeywordQuery(keyword);

            string pre = @"{ ""bool"": { ""should"": [";
            string post = @"], ""minimum_should_match"": null, ""disable_coord"": null,""_name"": null,""boost"": null } }";
            string expected = pre + string.Join(',', expectedFullTextQuery) + post;

            ElasticTools.AssertQueryJson(expected, actual);
        }

        #endregion

        #region GetFullTextQuery

        public static IEnumerable<object[]> GetFullTextQueryScenarioData => new[] {
            // Multiple fields, multiple match types
            new object[] {
                "testkeyword",
                new R4RAPIOptions.FullTextFieldConfig[]
                {
                    new R4RAPIOptions.FullTextFieldConfig
                    {
                        FieldName = "testfield1",
                        Boost = 1,
                        MatchTypes = new string[] { "common", "match" }
                    },
                    new R4RAPIOptions.FullTextFieldConfig
                    {
                        FieldName = "testfield2",
                        Boost = 1,
                        MatchTypes = new string[] { "common" }
                    },
                    new R4RAPIOptions.FullTextFieldConfig
                    {
                        FieldName = "testfield3",
                        Boost = 1,
                        MatchTypes = new string[] { "common", "match", "match_phrase" }
                    },
                    new R4RAPIOptions.FullTextFieldConfig
                    {
                        FieldName = "testfield4",
                        Boost = 1,
                        MatchTypes = new string[] { "match" }
                    }
                },
                new string[]{
                    @"
                        { ""common"": { ""testfield1"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield1"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""common"": { ""testfield2"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""common"": { ""testfield3"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield3"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield3"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } },
                        { ""match"": { ""testfield4"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        
                    "
                }
            }
        };

        [Theory, MemberData(nameof(GetFullTextQueryScenarioData))]
        public void GetFullTextQuery_Scenarios(string keyword, R4RAPIOptions.FullTextFieldConfig[] fields, string[] expectedFullTextQuery)
        {
            var fullTextQuery = this._junkSvc.TEST_GetFullTextQuery(keyword, fields);
            BoolQuery actual = new BoolQuery
            {
                Should = fullTextQuery
            };

            string pre = @"{ ""bool"": { ""should"": [";
            string post = @"], ""minimum_should_match"": null, ""disable_coord"": null,""_name"": null,""boost"": null } }";
            string expected = pre + string.Join(',', expectedFullTextQuery) + post;

            ElasticTools.AssertQueryJson(expected, actual);
        }

        // Full text query with two fields, one with multiple match types
        [Fact]
        public void GetFullTextQuery_TwoFields()
        {
            var fullTextQuery = this._junkSvc.TEST_GetFullTextQuery(
                "testkeyword",
                new R4RAPIOptions.FullTextFieldConfig[]
                {
                    new R4RAPIOptions.FullTextFieldConfig
                    {
                        FieldName = "testfield1",
                        Boost = 1,
                        MatchTypes = new string[] { "common", "match" }
                    },
                    new R4RAPIOptions.FullTextFieldConfig
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
                            { ""common"": { ""testfield1"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                            { ""match"": { ""testfield1"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                            { ""common"": { ""testfield2"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
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

        // Data for testing field query building
        public static IEnumerable<object[]> GetQueryForFullTextFieldScenarioData => new[] {
            // Single MatchType
            new object[] {
                "testkeyword",
                new R4RAPIOptions.FullTextFieldConfig
                {
                    FieldName = "testfield",
                    Boost = 1,
                    MatchTypes = new string [] { "common" }
                },
                new string[]{
                    @"{ ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }"
                }
            },
            // Multiple MatchTypes
            new object[] {
                "testkeyword",
                new R4RAPIOptions.FullTextFieldConfig
                {
                    FieldName = "testfield",
                    Boost = 1,
                    MatchTypes = new string [] { "common", "match" }
                },
                new string[]{
                    @"
                        { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } }
                    "
                }
            },
            // Multiple MatchTypes
            new object[] {
                "testkeyword",
                new R4RAPIOptions.FullTextFieldConfig
                {
                    FieldName = "testfield",
                    Boost = 1,
                    MatchTypes = new string [] { "common", "match", "match_phrase" }
                },
                new string[]{
                    @"
                        { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } },
                        { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } }
                    "
                }
            }
        };

        // Queries for one field, multiple scenarios
        [Theory, MemberData(nameof(GetQueryForFullTextFieldScenarioData))]
        public void GetQueryForFullTextField_Scenarios(string keyword, R4RAPIOptions.FullTextFieldConfig field, string[] expectedFullTextFieldQueries)
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

        // Queries for one field with multiple match types
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
                            { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } },
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

        // Queries for one field with single match type
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
                            { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
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

        // MatchType "common"
        [Fact]
        public void GetQueryForMatchType_Common()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "common");
            var expectedStr = @" 
                { ""common"": { ""testfield"": { ""query"": ""testkeyword"", ""cutoff_frequency"": 1.0, ""low_freq_operator"": ""and"", ""boost"": 1.0 } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        // MatchType "match"
        [Fact]
        public void GetQueryForMatchType_Match()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "match");
            var expectedStr = @" 
                { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0 } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        // MatchType "match_phrase"
        [Fact]
        public void GetQueryForMatchType_MatchPhrase()
        {
            var query = this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "match_phrase");
            var expectedStr = @" 
                { ""match"": { ""testfield"": { ""query"": ""testkeyword"", ""boost"": 1.0, ""type"": ""phrase"" } } }
            ";

            ElasticTools.AssertQueryJson(expectedStr, query);
        }

        // MatchType invalid
        [Fact]
        public void GetQueryForMatchType_GetInvalidMatchType()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                this._junkSvc.TEST_GetQueryForMatchType("testfield", "testkeyword", 1, "invalid");
            });
        }

        // MatchType none
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
