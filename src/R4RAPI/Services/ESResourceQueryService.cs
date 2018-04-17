using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    public class ESResourceQueryService
    {
        /// <summary>
        /// Gets a resource from the API via its ID.
        /// </summary>
        /// <param name="id">The ID of the resource</param>
        /// <returns>The resource</returns>
        public ResourceQueryResult Get(string id)
        {

            ResourceQueryResult resResult = null;

            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("The resource identifier is null or an empty string");
            }

            //Get the HTTP response content from GET request
            //HttpContent httpContent = ReturnGetRespContent("clinical-trial", id);
            //rtnTrial = httpContent.ReadAsAsync<ClinicalTrial>().Result;

            // If there are no sites for a trial in the API, the ClinicalTrial object will return Sites == null.  
            // If this is the case, make ClinicalTrial.Sites an empty list.
            //if (rtnTrial.Sites == null)
            //{
            //    rtnTrial.Sites = new List<ClinicalTrial.StudySite>();
            //}

            return resResult;
        }

        /// <summary>
        /// Calls the search endpoint (/resources) of the R4R API
        /// </summary>
        /// <param name="query">Query parameters (optional)</param>
        /// <param name="size">Number of results to return (optional)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="includeFields">Fields to include (optional)</param>
        /// <param name="excludeFields">Fields to exclude (optional)</param>        
        /// <returns>Resource query result</returns>
        public ResourceQueryResult List(
            ResourceQuery query,
            int size = 10,
            int from = 0,
            string[] includeFields = null,
            string[] excludeFields = null
            )
        {
            ResourceQueryResult resResults = null;

            //Handle Null include/exclude field
            query = query ?? new ResourceQuery();
            includeFields = includeFields ?? new string[0];
            excludeFields = excludeFields ?? new string[0];

            //Make a copy of our search query so that we don't muck with the original.
            //(The query will need to contain the size, from, etc
            //JObject requestBody = (JObject)query.DeepClone();
            //requestBody.Add(new JProperty("size", size));
            //requestBody.Add(new JProperty("from", from));

            //if (includeFields.Length > 0)
            //{
            //    requestBody.Add(new JProperty("include", includeFields));
            //}

            //if (excludeFields.Length > 0)
            //{
            //    requestBody.Add(new JProperty("exclude", excludeFields));
            //}

            //Get the HTTP response content from POST request
            //HttpContent httpContent = ReturnPostRespContent("clinical-trials", requestBody);
            //rtnResults = httpContent.ReadAsAsync<ClinicalTrialsCollection>().Result;

            return resResults;
        }
    }
}
