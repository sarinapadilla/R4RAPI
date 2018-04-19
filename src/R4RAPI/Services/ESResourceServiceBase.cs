using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using R4RAPI.Models;


namespace R4RAPI.Services
{

    /// <summary>
    /// Base class for all ElasticSearch based ResourceServices
    /// </summary>
    public abstract class ESResourceServiceBase
    {
        protected readonly IElasticClient _elasticClient;
        protected readonly R4RAPIOptions _apiOptions;
        protected readonly ILogger _logger;

        public ESResourceServiceBase(IElasticClient client, IOptions<R4RAPIOptions> apiOptionsAccessor, ILogger logger)
        {
            this._elasticClient = client;
            this._apiOptions = apiOptionsAccessor.Value;
            this._logger = logger;
        }
    }
}
