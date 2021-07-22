using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Neuroglia.AsyncApi;
using Neuroglia.AsyncApi.Models.Bindings.WebSockets;
using TodoApi.Controllers;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddAsyncApiGeneration(options =>
            {
                options
                .WithMarkupType<WeatherForecastController>()
                .UseDefaultConfiguration(asyncapi =>
                {
                    asyncapi.UseServer("WebSocket", server => server
                        .WithUrl(new Uri("http://127.0.0.1:8081/ws/weather"))
                        .WithProtocol(AsyncApiProtocols.Ws)
                        .UseBinding(new WsServerBinding())
                        );
                });
            });
            services.AddAsyncApiUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseAsyncApiGeneration();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<WeatherForecastHub>("/ws/weather");
                endpoints.MapControllers();
            });
        }
    }
}
