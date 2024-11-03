using Microsoft.Extensions.Configuration;

namespace Cofoundry.Plugins.Azure.Tests;

public static class ConfigurationHelper
{
    public static IConfigurationRoot GetConfigurationRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        return new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
