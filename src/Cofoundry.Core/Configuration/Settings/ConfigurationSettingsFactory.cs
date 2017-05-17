using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Validation;
using Microsoft.Extensions.Options;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// An InjectionFactory for allowing injection and validation of configuration settings
    /// without having to request IOptions directly.
    /// </summary>
    /// <typeparam name="TSettings">Type of settings object to instantiate</typeparam>
    public class ConfigurationSettingsFactory<TSettings> : IConfigurationSettingsFactory<TSettings> where TSettings : class, IConfigurationSettings, new()
    {
        #region constructor

        private readonly IResolutionContext _resolutionContext;
        private readonly IModelValidationService _modelValidationService;

        public ConfigurationSettingsFactory(
            IResolutionContext resolutionContext,
            IModelValidationService modelValidationService
            )
        {
            _resolutionContext = resolutionContext;
            _modelValidationService = modelValidationService;
        }

        #endregion

        #region public

        /// <summary>
        /// Creates an instance of a settings objects, extracting setting values from
        /// a configuration source.
        /// </summary>
        public TSettings Create()
        {
            var settingsOptions = _resolutionContext.Resolve<IOptions<TSettings>>();
            var settings = settingsOptions.Value;

            var errors = _modelValidationService.GetErrors(settings);

            if (!EnumerableHelper.IsNullOrEmpty(errors))
            {
                throw new InvalidConfigurationException(typeof(TSettings).Name, errors);
            }

            return settings;
        }

        #endregion
    }
}
