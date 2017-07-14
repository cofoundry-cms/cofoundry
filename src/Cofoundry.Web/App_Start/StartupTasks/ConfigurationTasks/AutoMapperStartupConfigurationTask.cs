using Cofoundry.Core.AutoMapper;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Sets up AutoMapper to use DI and adds some basic converters
    /// </summary>
    public class AutoMapperStartupConfigurationTask : IStartupConfigurationTask
    {
        private readonly IAutoMapBootstrapper _autoMapBootstrapper;

        public AutoMapperStartupConfigurationTask(
            IAutoMapBootstrapper autoMapBootstrapper
            )
        {
            _autoMapBootstrapper = autoMapBootstrapper;
        }

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Configure(IApplicationBuilder app)
        {
            _autoMapBootstrapper.Configure(cfg =>
            {
                cfg
                    .AddHtmlStringConverters();

                    // TODO: What to do about this? Rip out automapper?
                    //.ConstructServicesUsing(app.ApplicationServices.GetService);
            });
        }
    }
}