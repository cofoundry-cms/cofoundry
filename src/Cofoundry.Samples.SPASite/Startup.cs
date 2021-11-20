using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Cofoundry.Web;
using Microsoft.AspNetCore.Hosting;
using VueCliMiddleware;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices;

namespace Cofoundry.Samples.SPASite
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services
                .AddControllersWithViews()
                .AddCofoundry(Configuration)
                ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSpaStaticFiles();
            app.UseCofoundry();

            // Un-comment this to run the vue cli automatically when debugging
            // You'll need to install the vue cli, see https://cli.vuejs.org/guide/installation.html 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapToVueCliProxy(
                    "{*path}",
                    new SpaOptions { SourcePath = "ClientApp" },
                    npmScript: null, //(System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                    regex: "Compiled successfully",
                    forceKill: true
                    );
            });
        }
    }
}
