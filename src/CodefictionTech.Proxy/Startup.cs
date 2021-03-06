using System;
using System.Net;
using System.Net.Http;
using CodefictionTech.Proxy.Core.Extensions;
using CodefictionTech.Proxy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodefictionTech.Proxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddProxyWithCustomHttpService<CodefictionHttpRequestService>(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        UseCookies = false,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    };

                    return handler;
                })
                .AddResponseCompression()
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });

            string proxyUrl = Configuration["ProxyUrl"];

            app.UseResponseCompression()
                .UseHttpsRedirection()
                .UseMvc()
                .UseWebSockets()
                .RunProxy(new Uri(proxyUrl));
        }
    }
}
