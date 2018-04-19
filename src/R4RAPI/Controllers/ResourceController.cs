using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using R4RAPI.Models;
using R4RAPI.Services;
using Newtonsoft.Json;

namespace R4RAPI.Controllers
{
    [Produces("application/json")]
    [Route("resource")]
    public class ResourceController : Controller
    {
        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IResourceQueryService _queryService;

        public ResourceController(IHostingEnvironment environment, ILogger<ResourcesController> logger, IResourceQueryService queryService)
        {
            _environment = environment;
            _logger = logger;
            _queryService = queryService;
        }

        [HttpGet("{id}")]
        public Resource GetById(int id)
        {
            Resource result = _queryService.Get(id.ToString());

            if(result == null)
            {
                _logger.LogError("Could not fetch resource for ID " + id);
                throw new Exception("Could not fetch resource for ID " + id);
            }

            return result;
        }
    }
}