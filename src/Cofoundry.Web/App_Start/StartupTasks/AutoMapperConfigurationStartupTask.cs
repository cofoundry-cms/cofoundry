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
    public class AutoMapperConfigurationStartupTask : IStartupTask
    {
        private readonly IAutoMapBootstrapper _autoMapBootstrapper;

        public AutoMapperConfigurationStartupTask(
            IAutoMapBootstrapper autoMapBootstrapper
            )
        {
            _autoMapBootstrapper = autoMapBootstrapper;
        }

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IApplicationBuilder app)
        {
            _autoMapBootstrapper.Configure(cfg =>
            {
                cfg
                    .AddHtmlStringConverters()
                    // TODO: What to do about this? Rip out automapper?
                    .ConstructServicesUsing(IckyDependencyResolution.ResolveFromMvcContext);
            });
        }
    }
}