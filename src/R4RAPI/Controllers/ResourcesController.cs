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
using Newtonsoft.Json;

namespace R4RAPI.Controllers
{
    [Produces("application/json")]
    [Route("resources")]
    public class ResourcesController : Controller
    {
        private static readonly string _file = "R4RData.txt";
        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly ElasticSearchOptions _esOptions;

        public ResourcesController(IHostingEnvironment environment, ILogger<ResourcesController> logger, IOptions<ElasticSearchOptions> esOptionsAccessor)
        {
            _environment = environment;
            _logger = logger;
            _esOptions = esOptionsAccessor.Value;
        }

        [HttpGet]
        public ResourceResults GetAll()
        {
            _logger.LogDebug(_esOptions.MaximumRetries.ToString());

            string webRoot = _environment.WebRootPath;
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
            }
        }
    }
}