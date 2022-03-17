using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Configuration;

/// <summary>
/// An InjectionFactory for transforming application configuration files into settings objects.
/// </summary>
/// <typeparam name="TSettings">Type of settings object to instantiate</typeparam>
public interface IConfigurationSettingsFactory<TSettings> : IInjectionFactory<TSettings> where TSettings : class, IConfigurationSettings, new()
{
}
