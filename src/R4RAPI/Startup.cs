using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R4RAPI.Models;
using Nest;
using Elasticsearch.Net;

namespace R4RAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            services.Configure<ElasticsearchOptions>(Configuration.GetSection("Elasticsearch"));
            services.Configure<R4RAPIOptions>(Configuration.GetSection("R4RAPI"));

            // This will inject an IElasticClient using our configuration into any
            // controllers that take an IElasticClient parameter into its constructor.
            //
            // AddTransient means that it will instantiate a new instance of our client
            // for each instance of the controller.  So the function below will be called
            // on each request.   
            services.AddTransient<IElasticClient>(p => {

                // Get the ElasticSearch credentials.
                string username = Configuration["Elasticsearch:Userid"];
                string password = Configuration["Elasticsearch:Password"];

                //Get the ElasticSearch servers that we will be connecting to.
                List<Uri> uris = GetServerUriList();

                // Create the connection pool, the SniffingConnectionPool will 
                // keep tabs on the health of the servers in the cluster and
                // probe them to ensure they are healthy.  This is how we handle
                // redundancy and load balancing.
                var connectionPool = new SniffingConnectionPool(uris);

                //Return a new instance of an ElasticClient with our settings
                ConnectionSettings settings = new ConnectionSettings(connectionPool);

                //Let's only try and use credentials if the username is set.
                if (!string.IsNullOrWhiteSpace(username))
                {
                    settings.BasicAuthentication(username, password);
                }

                return new ElasticClient(settings);
            });


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        /// <summary>
        /// Retrieves a list of Elasticsearch server URIs from the configuration's Elasticsearch:Servers setting. 
        /// </summary>
        /// <returns>Returns a list of one or more Uri objects representing the configured set of Elasticsearch servers</returns>
        /// <remarks>
        /// The configuration's Elasticsearch:Servers property is required to contain URIs for one or more Elasticsearch servers.
        /// Each URI must include a protocol (http or https), a server name, and optionally, a port number.
        /// Multiple URIs are separated by a comma.  (e.g. "https://fred:9200, https://george:9201, https://ginny:9202")
        /// 
        /// Throws ConfigurationException if no servers are configured.
        ///
        /// Throws UriFormatException if any of the configured server URIs are not formatted correctly.
        /// </remarks>
        private List<Uri> GetServerUriList()
        {
            List<Uri> uris = new List<Uri>();

            string serverList = Configuration["Elasticsearch:Servers"];
            if (!String.IsNullOrWhiteSpace(serverList))
            {
                // Convert the list of servers into a list of Uris.
                string[] names = serverList.Split(',');
                uris.AddRange(names.Select(server => new Uri(server)));
            }
            else
            {
                throw new Exception("No servers configured");
            }

            return uris;
        }
    }
}
