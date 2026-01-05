using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Plugins.Azure;

/// <summary>
/// Extension methods for <see cref="IContainerConfigurationHelper"/>.
/// </summary>
public static class IContainerRegisterExtensions
{
    extension(IContainerConfigurationHelper helper)
    {
        /// <summary>
        /// Indicates whether the Cofoundry.Plugins.Azure plugin has been
        /// disabled via config settings.
        /// </summary>
        /// <returns>True if the azure plugin is disabled; otherwise false.</returns>
        public bool IsAzurePluginEnabled()
        {
            return !helper.GetValue<bool>("Cofoundry:Plugins:Azure:Disabled");
        }
    }
}
