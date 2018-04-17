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
using Newtonsoft.Json;

namespace R4RAPI.Controllers
{
    [Produces("application/json")]
    [Route("resource")]
    public class ResourceController : Controller
    {
        private static readonly string _file = "R4RData.txt";
        private IHostingEnvironment _environment;
        private readonly ILogger _logger;

        public ResourceController(IHostingEnvironment environment, ILogger<ResourcesController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public Resource GetById(int id)
        {
            ResourceResults results = GetAllResources();
            Resource res = results.Results.FirstOrDefault(t => t.ID == id);
            if (res == null)
            {
                return null;
            }
            return res;
        }

        public ResourceResults GetAllResources()
        {
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