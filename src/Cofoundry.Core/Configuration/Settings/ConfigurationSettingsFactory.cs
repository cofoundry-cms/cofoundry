using Cofoundry.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// An InjectionFactory for allowing injection and validation of configuration settings
    /// without having to request IOptions directly.
    /// </summary>
    /// <typeparam name="TSettings">Type of settings object to instantiate</typeparam>
    public class ConfigurationSettingsFactory<TSettings> : IConfigurationSettingsFactory<TSettings> where TSettings : class, IConfigurationSettings, new()
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModelValidationService _modelValidationService;

        public ConfigurationSettingsFactory(
            IServiceProvider serviceProvider,
            IModelValidationService modelValidationService
            )
        {
            _serviceProvider = serviceProvider;
            _modelValidationService = modelValidationService;
        }

        /// <summary>
        /// Creates an instance of a settings objects, extracting setting values from
        /// a configuration source.
        /// </summary>
        public TSettings Create()
        {
            var settingsOptions = _serviceProvider.GetRequiredService<IOptions<TSettings>>();
            var settings = settingsOptions.Value;

            if (settings is IFeatureEnableable featureEnableable && !featureEnableable.Enabled)
            {
                // feature is disabled, so skip validation.
                return settings;
            }

            var errors = _modelValidationService.GetErrors(settings);

            if (!EnumerableHelper.IsNullOrEmpty(errors))
            {
                throw new InvalidConfigurationException(typeof(TSettings), errors);
            }

            return settings;
        }
    }
}
