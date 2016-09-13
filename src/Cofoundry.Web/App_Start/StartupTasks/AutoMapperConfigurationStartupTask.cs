using Cofoundry.Core.AutoMapper;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public void Run(IAppBuilder app)
        {
            _autoMapBootstrapper.Configure(cfg =>
            {
                cfg
                    .AddHtmlStringConverters()
                    .ConstructServicesUsing(IckyDependencyResolution.ResolveFromMvcContext);
            });
        }
    }
}