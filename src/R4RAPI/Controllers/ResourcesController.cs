using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
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
    [Produces("application/json")]
    [Route("resources")]
    public class ResourcesController : Controller
    {
        //private static readonly string _file = "R4RData.txt";
        public static readonly string[] DefaultFacets = {
            // These R4R facets appear in results:
            ""
            // These R4R facets DO NOT appear in results:
            /// 
        };

        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly ElasticSearchOptions _esOptions;
        private readonly ESResourceQueryService _queryService;

        public ResourcesController(IHostingEnvironment environment, ILogger<ResourcesController> logger, IOptions<ElasticSearchOptions> esOptionsAccessor, ESResourceQueryService queryService)
        {
            _environment = environment;
            _logger = logger;
            _esOptions = esOptionsAccessor.Value;
            _queryService = queryService;

        }

        [HttpGet]
        public ResourceResults GetAll(
            [FromQuery(Name = "q")] string keyword,
            [FromQuery(Name = "toolType.type")] string[] toolTypes = null,
            [FromQuery(Name = "toolType.subtype")] string[] subTypes = null,
            [FromQuery(Name = "researchAreas")] string[] researchAreas = null,
            [FromQuery(Name = "researchTypes")] string[] researchTypes = null,
            [FromQuery(Name = "include")] string[] includeFields = null,
            [FromQuery(Name = "includeFacets")] string[] includeFacets = null,
            [FromQuery] int size = 20,
            [FromQuery] int from = 0
        )
        {
            // Set default values for params
            if(IsNullOrEmpty(includeFields))
            {
                includeFields = new string[] { };
            }

            if(IsNullOrEmpty(includeFacets))
            {
                includeFacets = DefaultFacets;
            }

            // TODO: Validate query params here
            // 1. Cause error if subToolType exists, but no toolType
            if(IsNullOrEmpty(toolTypes) && !IsNullOrEmpty(subTypes))
            {
                _logger.LogError("Cannot have subtype without tooltype.", subTypes);
            }
            // 2. Cause error if multiple toolTypes exist
            if (toolTypes != null && toolTypes.Length > 1)
            {
                _logger.LogError("Cannot have multiple tooltype.", toolTypes);
            }

            // Build query object using params
            ResourceQuery resQuery = new ResourceQuery();

            if(!string.IsNullOrWhiteSpace(keyword))
            {
                resQuery.Keyword = keyword;
            }

            if(!IsNullOrEmpty(toolTypes))
            {
                resQuery.Filters.Add("toolTypes.type", toolTypes);
            }

            if(!IsNullOrEmpty(subTypes))
            {
                resQuery.Filters.Add("toolTypes.subtype", subTypes);
            }

            if(!IsNullOrEmpty(researchAreas))
            {
                resQuery.Filters.Add("researchAreas", researchAreas);
            }

            if (!IsNullOrEmpty(researchTypes))
            {
                resQuery.Filters.Add("researchTypes", researchTypes);
            }

            if (!IsNullOrEmpty(includeFields))
            {
                resQuery.Filters.Add("include", includeFields);
            }

            if (!IsNullOrEmpty(includeFacets))
            {
                resQuery.Filters.Add("includeFacets", includeFacets);
            }

            ResourceQueryResult queryResults = null;

            if(!string.IsNullOrWhiteSpace(resQuery.Keyword))
            {
                queryResults = _queryService.Query(resQuery, size, from, includeFields);
            }

            return new ResourceResults();

            /*string webRoot = _environment.WebRootPath;
            string filePath = Path.Combine(webRoot, _file);

            try
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    ResourceResults results = JsonConvert.DeserializeObject<ResourceResults>(json);
                    return results;
                }
            }
            catch
            {
                //log.ErrorFormat("GetAll(): Path {0} not found.", ex, filePath);
                return null;
            }*/
        }

        static bool IsNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }
    }
}