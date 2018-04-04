using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
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

        public ResourcesController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public ResourceResults GetAll()
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

            /*
            List<Resource> res = new List<Resource>();
            Resource resource = new Resource(12345);
            Resource resource2 = new Resource(67890);
            res.Add(resource);
            res.Add(resource2);
            */
        }
    }
}