using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Cofoundry.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Cofoundry.BasicTestSite
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
            // the default config from the starer template requires consent
            // but CF uses an essental cookie for login and therefore should
            // continue to work.
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddMvc()
                .AddCofoundry(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Https redirection / cookie policy and hsts is all non-cofoundry stuff
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseCofoundry();

        }
    }
}
