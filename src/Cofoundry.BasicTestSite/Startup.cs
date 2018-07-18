using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Cofoundry.Web;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
            services
                .AddMvc()
                .AddCofoundry(Configuration, cofoundryConfiguration =>
                {
                    cofoundryConfiguration.DefaultRequestCulture = new RequestCulture("en", "en");
                    cofoundryConfiguration.EnableLocalization = true;
                });

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCofoundry();
        }
    }
}
