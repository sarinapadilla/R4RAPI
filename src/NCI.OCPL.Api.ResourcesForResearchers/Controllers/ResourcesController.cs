using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCI.OCPL.Api.ResourcesForResearchers.Models;
using NCI.OCPL.Api.ResourcesForResearchers.Services;
using Newtonsoft.Json;

namespace NCI.OCPL.Api.ResourcesForResearchers.Controllers
{
    /// <summary>
    /// Controller for the Resources Endpoint
    /// </summary>
    [Produces("application/json")]
    [Route("resources")]
    public class ResourcesController : Controller
    {
        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly R4RAPIOptions _apiOptions;
        private readonly IResourceQueryService _queryService;
        private readonly IResourceAggregationService _aggService;
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Controllers.ResourcesController"/> class.
        /// </summary>
        /// <param name="environment">Environment.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="queryService">Query service.</param>
        /// <param name="aggService">Agg service.</param>
        public ResourcesController(
            IHostingEnvironment environment, 
            ILogger<ResourcesController> logger,
            IOptions<R4RAPIOptions> apiOptionsAccessor,
            IResourceQueryService queryService, 
            IResourceAggregationService aggService,
            IUrlHelper urlHelper)
        {
            _environment = environment;
            _logger = logger;
            _apiOptions = apiOptionsAccessor.Value;
            _queryService = queryService;
            _aggService = aggService;
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Searches all resources
        /// </summary>
        /// <returns>The Resources and available facets that match the query</returns>
        /// <param name="keyword">Full-text search keyword</param>
        /// <param name="toolTypes">One or more toolTypes keys to filter by</param>
        /// <param name="subTypes">One or more toolSubtypes keys to filter by</param>
        /// <param name="researchAreas">One or more researchArea keys to filter by</param>
        /// <param name="researchTypes">One or more researchTypes keys to filter by</param>
        /// <param name="docs">One or more Divisions, Offices, and Centers (DOCs) to filter by </param>
        /// <param name="includeFields">Resource fields to include. (When empty all are returned)</param>
        /// <param name="includeFacets">Available Facets to return. (When empty all are returned)</param>
        /// <param name="strSize">The number of resources to return</param>
        /// <param name="strFrom">The offset of the resources to return</param>
        [HttpGet]
        public async Task<ResourceResults> GetAll(
            [FromQuery(Name = "q")] string keyword = null,
            [FromQuery(Name = "toolTypes")] string[] toolTypes = null,
            [FromQuery(Name = "toolSubtypes")] string[] subTypes = null,
            [FromQuery(Name = "researchAreas")] string[] researchAreas = null,
            [FromQuery(Name = "researchTypes")] string[] researchTypes = null,
            [FromQuery(Name = "docs")] string[] docs = null,
            [FromQuery(Name = "include")] string[] includeFields = null,
            [FromQuery(Name = "includeFacets")] string[] includeFacets = null,
            [FromQuery(Name = "size")] string strSize = null,
            [FromQuery(Name = "from")] string strFrom = null
        )
        {
            // Validate query params here

            // 1. Throw error if subToolType exists, but no toolType
            if (IsNullOrEmpty(toolTypes) && !IsNullOrEmpty(subTypes))
            {
                _logger.LogError("Cannot have subtype without tooltype.", subTypes);
                throw new ArgumentException("Cannot have subtype without tooltype.");
            }

            // 2. Throw error if multiple toolTypes exist
            if (toolTypes != null && toolTypes.Length > 1)
            {
                _logger.LogError("Cannot have multiple tooltypes.", toolTypes);
                throw new ArgumentException("Cannot have multiple tooltypes.");
            }

            // 3. Throw error if size is invalid int
            int size;
            if (string.IsNullOrWhiteSpace(strSize))
            {
                size = 20;
            }
            else
            {
                int tmpInt = -1;
                if (int.TryParse(strSize.Trim(), out tmpInt))
                {
                    if (tmpInt == -1)
                    {
                        _logger.LogError($"Invalid size parameter: {strSize}.");
                        throw new APIErrorException(400, $"Bad request: Invalid size parameter {strSize}.");
                    }

                    size = tmpInt;
                }
                else
                {
                    _logger.LogError($"Invalid size parameter: {strSize}.");
                    throw new APIErrorException(400, $"Bad request: Invalid size parameter {strSize}.");
                }
            }

            // 4. Throw error if from is invalid int
            int from;
            if (string.IsNullOrWhiteSpace(strFrom))
            {
                from = 0;
            }
            else
            {
                int tmpInt = -1;
                if (int.TryParse(strFrom.Trim(), out tmpInt))
                {
                    if (tmpInt == -1)
                    {
                        _logger.LogError($"Invalid from parameter: {strFrom}.");
                        throw new APIErrorException(400, $"Bad request: Invalid from parameter {strFrom}.");
                    }

                    from = tmpInt;
                }
                else
                {
                    _logger.LogError($"Invalid from parameter: {strFrom}.");
                    throw new APIErrorException(400, $"Bad request: Invalid from parameter {strFrom}.");
                }
            }

            // Build resource query object using params
            ResourceQuery resourceQuery = new ResourceQuery();

            // Add keyword, if included in params
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                resourceQuery.Keyword = keyword;
            }

            // Add tool types, if included in params
            if (!IsNullOrEmpty(toolTypes))
            {
                resourceQuery.Filters.Add("toolTypes", toolTypes);
            }

            // Add tool subtypes, if included in params
            if (!IsNullOrEmpty(subTypes))
            {
                resourceQuery.Filters.Add("toolSubtypes", subTypes);
            }

            // Add research areas, if included in params
            if (!IsNullOrEmpty(researchAreas))
            {
                resourceQuery.Filters.Add("researchAreas", researchAreas);
            }

            // Add research types, if included in params
            if (!IsNullOrEmpty(researchTypes))
            {
                resourceQuery.Filters.Add("researchTypes", researchTypes);
            }

            // Add docs, if included in params
            if (!IsNullOrEmpty(docs))
            {
                resourceQuery.Filters.Add("docs", docs);
            }

            // Set default values for params
            //facetsBeingRequest is used because if we modify the original params
            //then the OriginalString of the response will show the default 
            //include facets.
            string[] facetsBeingRequested = new string[] { };
            if (IsNullOrEmpty(includeFacets))
            {
                facetsBeingRequested = GetDefaultIncludeFacets();
            }
            else if (!ValidateFacetList(includeFacets))
            {
                //TODO: Actually list the invalid facets
                _logger.LogError("Included facets in query are not valid.");
                throw new ArgumentException("Included facets in query are not valid.");
            } else {
                facetsBeingRequested = includeFacets;
            }

            string[] fieldsBeingRequested = new string[] { };
            if (IsNullOrEmpty(includeFields))
            {
                fieldsBeingRequested = GetDefaultIncludeFields();
            }
            else if (!ValidateFieldList(includeFields))
            {
                //TODO: Actually list the invalid fields
                _logger.LogError("Included fields in query are not valid.");
                throw new ArgumentException("Included fields in query are not valid.");
            }
            else
            {

                fieldsBeingRequested = includeFields;
            }

            //Create the results object
            ResourceResults results = new ResourceResults();
            bool noMatchedResults = true;

            //Now call query results & get aggs at the same time.
            await Task.WhenAll(
                Task.Run(async () =>
                {
                    // Perform query for resources (using params if they're given)
                    ResourceQueryResult queryResults = await _queryService.QueryResourcesAsync(resourceQuery, size, from, fieldsBeingRequested);

                    // Convert query results into ResourceResults
                    PageMetaData meta = new PageMetaData
                    {
                        TotalResults = queryResults.TotalResults,
                        From = queryResults.From,
                        OriginalQuery = _urlHelper.RouteUrl(new
                        {
                            size,
                            from,
                            q = keyword,
                            toolTypes,
                            toolSubtypes = subTypes,
                            researchAreas,
                            researchTypes,
                            docs,
                            include = includeFields,
                            //DO NOT USE facetsBeingRequested HERE. This is the original list provided by the user.
                            includeFacets
                        })
                    };
                    results.Meta = meta;
                    results.Results = queryResults.Results;

                    //While we may have asked for size 0, this will still tell us if the full query has results.
                    noMatchedResults = queryResults.TotalResults == 0;

                    if(from > queryResults.TotalResults)
                    {
                        throw new APIErrorException(400, $"Offset (from) value of {queryResults.TotalResults} is greater than the number of results.");
                    }
                }),
                Task.Run(async () =>
                {
                    // Perform query for facet aggregations (using includeFacets param if it's given, otherwise default facets to include from options)
                results.Facets = await GetFacets(facetsBeingRequested, resourceQuery); //Go back to sync now.
                })
            );

            //Multiselect facets will query all filters except thier own.  So,
            //you can have no search results returned, but still get a facet.
            //For example, /resources?researchAreas=chicken will actually return
            //all the facet items for researchAreas.  This code below removes
            //all facets when there are 0 results, unless the user has asked for
            //a query size of 0;
            if (noMatchedResults) {
                results.Facets = new Facet[] { };
            }

            return results;
        }

        static bool IsNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }

        /// <summary>
        /// Gets the default fields to include on returned results from the config
        /// </summary>
        /// <returns>A list of default fields</returns>
        private string[] GetDefaultIncludeFields()
        {
            return _apiOptions.AvailableFields;
        }

        /// <summary>
        /// Validates the given list of include fields using available fields from the config
        /// </summary>
        /// <returns>A boolean indicating whether the list of fields is valid</returns>
        /// <param name="includeFields">A list of facet names</param>
        private bool ValidateFieldList(string[] includeFields)
        {
            foreach (var fieldName in includeFields)
            {
                if (!_apiOptions.AvailableFields.Contains(fieldName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the default facets from the config
        /// </summary>
        /// <returns>A list of default facet names</returns>
        private string[] GetDefaultIncludeFacets()
        {
            // TODO: Remove RequiresFilter constraint once correct logic is implemented
            return _apiOptions.AvailableFacets.Where(f => f.Value.IncludeInDefault).Select(f => f.Key).ToArray();
        }

        /// <summary>
        /// Validates the given list of facets using available facets from the config
        /// </summary>
        /// <returns>A boolean indicating whether the list of facets is valid</returns>
        /// <param name="includeFacets">A list of facet names</param>
        private bool ValidateFacetList(string[] includeFacets)
        {
            foreach (var filterName in includeFacets)
            {
                if (!_apiOptions.AvailableFacets.Keys.Contains(filterName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Searches for all included facets
        /// </summary>
        /// <returns>An array of facets</returns>
        /// <param name="includeFacets">A list of facet names to include in our search</param>
        /// <param name="resourceQuery">The resource query (used for determining facet items)</param>
        private async Task<Facet[]> GetFacets(string[] includeFacets, ResourceQuery resourceQuery)
        {
            List<Facet> facets = new List<Facet>();

            //Get list of Tasks to await
            //Use an array based on recommendations from Concurrency in C# Cookbook
            //regarding possible issues with LINQ and async.
            var facetsToFetch = (from facet in includeFacets
                                 where ShouldFetchFacet(facet, resourceQuery)
                                 select FetchFacet(facet, resourceQuery)).ToArray();

            var returnedFacets = await Task.WhenAll(facetsToFetch);
            return returnedFacets.Where(f => f != null).ToArray();
        }

        /// <summary>
        /// Fetches a single facet asynchronously
        /// </summary>
        /// <returns>The facet.</returns>
        /// <param name="facetToFetch">Facet to fetch.</param>
        /// <param name="resourceQuery">Resource query.</param>
        private async Task<Facet> FetchFacet(string facetToFetch, ResourceQuery resourceQuery)
        {

            KeyLabelAggResult[] aggResults = await _aggService.GetKeyLabelAggregationAsync(facetToFetch, resourceQuery);
            if (aggResults != null && aggResults.Length > 0)
            {
                return TransformFacet(facetToFetch, resourceQuery, aggResults);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Transforms the string name and returned aggregate results into a Facet
        /// </summary>
        /// <returns>An facet</returns>
        /// <param name="facetToFetch">The name of the facet to fetch</param>
        /// <param name="resourceQuery">The resource query (used for determining currently-set filters)</param>
        /// <param name="aggResults">The list of aggregate results to transform to facet items</param>
        private Facet TransformFacet(string facetToFetch, ResourceQuery resourceQuery, KeyLabelAggResult[] aggResults)
        {
            R4RAPIOptions.FacetConfig config = _apiOptions.AvailableFacets[facetToFetch];

            IEnumerable<string> filters = new string[] { };
            if(resourceQuery.Filters.ContainsKey(config.FilterName))
            {
                filters = resourceQuery.Filters[config.FilterName];
            }

            Facet facet = new Facet
            {
                Param = config.FilterName,
                Title = config.Label,
                Items = aggResults.Select(fi =>
                    new FacetItem
                    {
                        Key = fi.Key,
                        Label = fi.Label,
                        Count = Convert.ToInt32(fi.Count),
                        Selected = filters.Contains(fi.Key)
                    }).ToArray()
            };

            if (config.FacetType == R4RAPIOptions.FacetTypes.Single && IsFilterSet(config.FilterName, resourceQuery))
            {
                facet.Items = facet.Items.Where(i => i.Selected).ToArray();
            }

            return facet;
        }

        /// <summary>
        /// Determines whether a facet should be fetched
        /// This returns false if a facet requires a different filter to be set, and that other filter isn't set
        /// </summary>
        /// <returns>A boolean, indicating whether to fetch the given facet</returns>
        /// <param name="facetName">A list of facet names to include in our search</param>
        /// <param name="resourceQuery">The resource query (used for determining currently-set filters)</param>
        private bool ShouldFetchFacet(string facetName, ResourceQuery resourceQuery)
        {
            R4RAPIOptions.FacetConfig config = _apiOptions.AvailableFacets[facetName];

            var filters = (!string.IsNullOrWhiteSpace(config.RequiresFilter) && resourceQuery.Filters.ContainsKey(config.RequiresFilter)) ? resourceQuery.Filters[config.RequiresFilter] : new string[] { };

            if (!string.IsNullOrWhiteSpace(config.RequiresFilter) && filters.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Checks if given filter is set in the query.
        /// </summary>
        /// <returns>A bool, indicating whether the filter is set</returns>
        /// <param name="filterName">The filter name to check</param>
        /// <param name="resourceQuery">The resource query</param>
        private bool IsFilterSet(string filterName, ResourceQuery resourceQuery)
        {
            return resourceQuery.Filters.ContainsKey(filterName) && resourceQuery.Filters[filterName].Length > 0;

        }
    }
}