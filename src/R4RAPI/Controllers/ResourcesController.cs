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
using R4RAPI.Models;
using R4RAPI.Services;
using Newtonsoft.Json;

namespace R4RAPI.Controllers
{
    /// <summary>
    /// Controller for the Resources Endpoint
    /// </summary>
    [Produces("application/json")]
    [Route("resources")]
    public class ResourcesController : Controller
    {
        //private static readonly string _file = "R4RData.txt";

        /// <summary>
        /// THIS SHOULD BE REMOVED IN LIEU OF THE CONFIG
        /// </summary>
        public static readonly string[] DefaultFacets = {
            // These R4R facets appear in results:
            ""
            // These R4R facets DO NOT appear in results:
        };

        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
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
            IResourceQueryService queryService, 
            IResourceAggregationService aggService,
            IUrlHelper urlHelper)
        {
            _environment = environment;
            _logger = logger;
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
        /// <param name="size">The number of resources to return</param>
        /// <param name="from">The offset of the resources to return</param>
        [HttpGet]
        public ResourceResults GetAll(
            [FromQuery(Name = "q")] string keyword = null,
            [FromQuery(Name = "toolTypes")] string[] toolTypes = null,
            [FromQuery(Name = "toolSubtypes")] string[] subTypes = null,
            [FromQuery(Name = "researchAreas")] string[] researchAreas = null,
            [FromQuery(Name = "researchTypes")] string[] researchTypes = null,
            [FromQuery(Name = "docs")] string[] docs = null,
            [FromQuery(Name = "include")] string[] includeFields = null,
            [FromQuery(Name = "includeFacets")] string[] includeFacets = null,
            [FromQuery] int size = 20,
            [FromQuery] int from = 0
        )
        {
            // Set default values for params
            if (IsNullOrEmpty(includeFields))
            {
                includeFields = new string[] { };
            }

            if (IsNullOrEmpty(includeFacets))
            {
                includeFacets = DefaultFacets;
            }

            // TODO: Validate query params here
            // 1. Cause error if subToolType exists, but no toolType
            if (IsNullOrEmpty(toolTypes) && !IsNullOrEmpty(subTypes))
            {
                _logger.LogError("Cannot have subtype without tooltype.", subTypes);
            }
            // 2. Cause error if multiple toolTypes exist
            if (toolTypes != null && toolTypes.Length > 1)
            {
                _logger.LogError("Cannot have multiple tooltype.", toolTypes);
            }

            // Build resource query object using params
            ResourceQuery resourceQuery = new ResourceQuery();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                resourceQuery.Keyword = keyword;
            }

            if (!IsNullOrEmpty(toolTypes))
            {
                resourceQuery.Filters.Add("toolTypes", toolTypes);
            }

            if (!IsNullOrEmpty(subTypes))
            {
                resourceQuery.Filters.Add("toolSubtypes", subTypes);
            }

            if (!IsNullOrEmpty(researchAreas))
            {
                resourceQuery.Filters.Add("researchAreas", researchAreas);
            }

            if (!IsNullOrEmpty(docs))
            {
                resourceQuery.Filters.Add("docs", docs);
            }

            if (!IsNullOrEmpty(researchTypes))
            {
                resourceQuery.Filters.Add("researchTypes", researchTypes);
            }

            if (!IsNullOrEmpty(includeFields))
            {
                resourceQuery.Filters.Add("include", includeFields);
            }

            if (!IsNullOrEmpty(includeFacets))
            {
                resourceQuery.Filters.Add("includeFacets", includeFacets);
            }

            // Perform query for resources (using params if they're given)
            ResourceQueryResult queryResults = _queryService.QueryResources(resourceQuery, size, from, includeFields);

            // Perform query for Research Areas and Research Types aggregations
            KeyLabelAggResult[] raAggResults = _aggService.GetKeyLabelAggregation("researchAreas", resourceQuery);
            KeyLabelAggResult[] rtAggResults = _aggService.GetKeyLabelAggregation("researchTypes", resourceQuery);
            KeyLabelAggResult[] ttAggResults = _aggService.GetKeyLabelAggregation("toolTypes", resourceQuery);

            // Convert query results into ResourceResults
            ResourceResults results = new ResourceResults();
            if(queryResults != null)
            {
                PageMetaData meta = new PageMetaData();
                meta.TotalResults = queryResults.TotalResults;
                meta.OriginalQuery = _urlHelper.RouteUrl(new {
                    size,
                    from,
                    q = keyword,
                    toolTypes,
                    toolSubtypes = subTypes,
                    researchAreas,
                    researchTypes,
                    docs,
                    include = includeFields,
                    includeFacets
                });
                results.Meta = meta;
                results.Results = queryResults.Results;
            }

            // Convert aggregations into facet items/facets
            List<Facet> facets = new List<Facet>();
            if (raAggResults != null)
            {
                Facet raFacet = new Facet();
                raFacet.Param = "researchAreas";
                raFacet.Title = "Research Areas";

                var raFacetItems = raAggResults.Select(i => new FacetItem { Key = i.Key, Label = i.Label, Count = Convert.ToInt32(i.Count) });
                raFacet.Items = raFacetItems.ToArray();

                facets.Add(raFacet);
            }
            
            if(rtAggResults != null)
            {
                Facet rtFacet = new Facet();
                rtFacet.Param = "researchTypes";
                rtFacet.Title = "Research Types";

                var rtFacetItems = rtAggResults.Select(i => new FacetItem { Key = i.Key, Label = i.Label, Count = Convert.ToInt32(i.Count) });
                rtFacet.Items = rtFacetItems.ToArray();

                facets.Add(rtFacet);
            }

            if (ttAggResults != null)
            {
                Facet ttFacet = new Facet();
                ttFacet.Param = "toolTypes";
                ttFacet.Title = "Tool Types";

                var ttFacetItems = ttAggResults.Select(i => new FacetItem { Key = i.Key, Label = i.Label, Count = Convert.ToInt32(i.Count) });
                ttFacet.Items = ttFacetItems.ToArray();

                facets.Add(ttFacet);
            }

            // Add facets to ResourceResults
            results.Facets = facets.ToArray<Facet>();

            return results;
        }

        static bool IsNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }
    }
}