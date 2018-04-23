using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R4RAPI.Models;
using R4RAPI.Services;
using Nest;
using Elasticsearch.Net;
using NSwag.AspNetCore;
using NJsonSchema;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace R4RAPI
{
    /// <summary>
    /// The API Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:R4RAPI.Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">Services.</param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            //This allows us to easily generate URLs to routes
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

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

            services.AddSingleton<IResourceQueryService, ESResourceQueryService>();
            services.AddSingleton<IResourceAggregationService, ESResourceAggregationService>();

            services.AddMvc();
        }

        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseStaticFiles();
            // Enable the Swagger UI middleware and the Swagger generator
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
            });

            // Allow use from anywhere.
            app.UseCors(builder => builder.AllowAnyOrigin());

            // This is equivelant to the old Global.asax OnError event handler.
            // It will handle any unhandled exception and return a status code to the
            // caller.  IF the error is of type APIErrorException then we will also return
            // a message along with the status code.  (Otherwise we )
            app.UseExceptionHandler(errorApp => {
                errorApp.Run(async context => {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var ex = error.Error;

                        //Unhandled exceptions may not be sanitized, so we will not
                        //display the issue.
                        string message = "Errors have occurred.  Type: " + ex.GetType().ToString();

                        //Our own exceptions should be sanitized enough.                        
                        if (ex is APIErrorException)
                        {
                            context.Response.StatusCode = ((APIErrorException)ex).HttpStatusCode;
                            message = ex.Message;
                        }

                        byte[] contents = Encoding.UTF8.GetBytes(new ErrorMessage()
                        {
                            Message = message
                        }.ToString());

                        await context.Response.Body.WriteAsync(contents, 0, contents.Length);
                    }
                });
            });

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
