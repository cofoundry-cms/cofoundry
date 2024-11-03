using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Plugins.Azure;

public static class IContainerRegisterExtensions
{
    /// <summary>
    /// Indicates whether the Cofoundry.Plugins.Azure plugin has been
    /// disabled via config settings.
    /// </summary>
    /// <returns>True if the azure plugin is disabled; otherwise false.</returns>
    public static bool IsAzurePluginEnabled(this IContainerConfigurationHelper helper)
    {
        return !helper.GetValue<bool>("Cofoundry:Plugins:Azure:Disabled");
    }
}
