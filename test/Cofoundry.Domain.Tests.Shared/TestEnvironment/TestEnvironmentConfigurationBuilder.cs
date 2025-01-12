using Microsoft.Extensions.Configuration;
namespace Cofoundry.Domain.Tests.Shared;

/// <summary>
/// Helper to build an <see cref="IConfiguration"/> instance
/// for use in a test environment.
/// </summary>
public static class TestEnvironmentConfigurationBuilder
{
    /// <summary>
    /// Builds an <see cref="IConfiguration"/> instance for a test
    /// applicationwith a standard set of sources including appsettings
    /// file, user secrets and environment variables.
    /// </summary>
    /// <param name="additionalConfiguration">
    /// Any additional configuration to apply to the end of the builder
    /// chain. Typically used to add settings determined at startup such
    /// as test container connection strings.
    /// </param>
    public static IConfiguration BuildConfiguration(
        Action<IConfigurationBuilder>? additionalConfiguration = null
        )
    {
        var builder = new ConfigurationBuilder();
        var currentDirectory = Directory.GetCurrentDirectory();
        builder.SetBasePath(currentDirectory);

        AddConfiguration(builder, additionalConfiguration);

        return builder.Build();
    }

    /// <summary>
    /// Adds a standard set of configuration sources to an existing
    /// <paramref name="builder"/> including appsettings files
    /// and environment variables.
    /// </summary>
    /// <param name="additionalConfiguration">
    /// Any additional configuration to apply to the end of the builder
    /// chain. Typically used to add settings determined at startup such
    /// as test container connection strings.
    /// </param>
    public static void AddConfiguration(
        IConfigurationBuilder builder,
        Action<IConfigurationBuilder>? additionalConfiguration = null
        )
    {
        builder
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables();

        additionalConfiguration?.Invoke(builder);
    }
}
