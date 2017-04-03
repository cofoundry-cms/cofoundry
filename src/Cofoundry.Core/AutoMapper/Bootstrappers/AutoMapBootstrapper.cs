using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.AutoMapper
{
    /// <summary>
    /// AutoMapper registration and initialization.
    /// </summary>
    public class AutoMapBootstrapper : IAutoMapBootstrapper
    {
        private readonly IEnumerable<Profile> _autoMapProfiles;

        public AutoMapBootstrapper(
            IEnumerable<Profile> autoMapProfiles
            )
        {
            _autoMapProfiles = autoMapProfiles;
        }

        /// <summary>
        /// Configures AutoMapper by detecting and registering
        /// any classes inheriting from AutoMaper.Profile
        /// </summary>
        public void Configure()
        {
            Configure(null);
        }
            
        /// <summary>
        /// Configures AutoMapper by detecting and registering
        /// any classes inheriting from AutoMaper.Profile
        /// </summary>
        /// <param name="configFn">Optional configuration method to call after the profiles have been registered.</param>
        public void Configure(Action<IMapperConfigurationExpression> configFn = null)
        {
            Mapper.Initialize(cfg =>
            {
                if (_autoMapProfiles != null)
                {
                    foreach (var profile in _autoMapProfiles)
                    {
                        cfg.AddProfile(profile);
                    }
                }

                if (configFn != null)
                {
                    configFn(cfg);
                }
            });
        }
    }
}
