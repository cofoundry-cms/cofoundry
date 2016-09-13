using AutoMapper;
using System;

namespace Cofoundry.Core.AutoMapper
{
    /// <summary>
    /// AutoMapper registration and initialization.
    /// </summary>
    public interface IAutoMapBootstrapper
    {
        /// <summary>
        /// Configures AutoMapper by detecting and registering
        /// any classes inheriting from AutoMaper.Profile
        /// </summary>
        void Configure();

        /// <summary>
        /// Configures AutoMapper by detecting and registering
        /// any classes inheriting from AutoMaper.Profile
        /// </summary>
        /// <param name="configFn">Optional configuration methdo to call after the profiles have been registered.</param>
        void Configure(Action<IMapperConfigurationExpression> configFn);

    }
}
