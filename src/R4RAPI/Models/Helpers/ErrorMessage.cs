using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace R4RAPI.Models
{
    /// <summary>
    /// Represents a Error Message to be returned to the client
    /// </summary>
    public class ErrorMessage
{
        /// <summary>
        /// The message to display 
        /// </summary>
        /// <returns></returns>
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
