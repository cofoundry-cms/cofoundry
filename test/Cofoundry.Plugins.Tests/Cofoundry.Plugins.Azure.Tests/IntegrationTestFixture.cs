using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Plugins.Azure.Internal;
using Microsoft.Extensions.Configuration;
using Testcontainers.Azurite;
using Xunit;

namespace Cofoundry.Plugins.Azure.Tests;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly AzureSettings _azureSettings;
    private AzuriteContainer? _azuriteContainer;

    public IntegrationTestFixture()
    {
        var configurationRoot = TestEnvironmentConfigurationBuilder.BuildConfiguration();
        var section = configurationRoot.GetSection("Cofoundry:Plugins:Azure");
        var settings = new AzureSettings();

        section.Bind(settings);

        _azureSettings = settings;
    }

    public async ValueTask InitializeAsync()
    {
        await InitializeAzuriteContainer();
        await ClearAllFiles();
    }

    private async Task InitializeAzuriteContainer()
    {
        if (string.IsNullOrEmpty(_azureSettings.BlobStorageConnectionString))
        {
            _azuriteContainer = new AzuriteBuilder()
                .WithCommand("--skipApiVersionCheck")
                .Build();
            await _azuriteContainer.StartAsync();
            _azureSettings.BlobStorageConnectionString = _azuriteContainer.GetConnectionString();
        }
    }

    /// <summary>
    /// Unless we're using a completely transient instance, we'll need to
    /// clean up any old test files that have been accumulated.
    /// </summary>
    private async Task ClearAllFiles()
    {
        var service = CreateFileStoreService();
        var client = service.GetServiceClient();

        await foreach (var container in client.GetBlobContainersAsync())
        {
            await client.DeleteBlobContainerAsync(container.Name);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_azuriteContainer != null)
        {
            await _azuriteContainer.DisposeAsync();
        }
    }

    public AzureBlobFileService CreateFileStoreService()
    {
        return new AzureBlobFileService(_azureSettings);
    }
}
