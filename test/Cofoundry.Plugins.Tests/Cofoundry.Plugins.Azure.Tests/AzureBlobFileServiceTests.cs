using System.Globalization;
using System.Text;
using Cofoundry.Core;
using Cofoundry.Plugins.Azure.Internal;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cofoundry.Plugins.Azure.Tests;

public class AzureBlobFileServiceTests : IAsyncLifetime
{
    private readonly AzureSettings _azureSettings;
    private static readonly string TEST_RUN_ID = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

    public AzureBlobFileServiceTests()
    {
        // TODO: make this config neater
        var configurationRoot = ConfigurationHelper.GetConfigurationRoot();
        var section = configurationRoot.GetSection("Cofoundry:Plugins:Azure");
        var settings = new AzureSettings();

        section.Bind(settings);

        _azureSettings = settings;
    }

    public async Task InitializeAsync()
    {
        var service = CreateFileStoreService();
        var client = service.GetServiceClient();

        await foreach (var container in client.GetBlobContainersAsync())
        {
            await client.DeleteBlobContainerAsync(container.Name);
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ClearContainer_IfMultipleFile_DoesClear()
    {
        var uniqueData = MakeContainerName(nameof(ClearContainer_IfMultipleFile_DoesClear));
        var containerName = uniqueData;
        var directoryName = "sub/";
        var fileName = GetFileName(uniqueData);
        var numFiles = 5;

        var service = CreateFileStoreService();

        // Setup files
        for (var i = 1; i < numFiles; i++)
        {
            // in root
            using (var stream = GetFileStream(containerName))
            {
                await service.CreateAsync(containerName, fileName + i, stream);
            }

            // in sub directory
            using (var stream = GetFileStream(uniqueData))
            {
                await service.CreateAsync(containerName, directoryName + fileName + i, stream);
            }
        }

        // Act

        await service.ClearContainerAsync(containerName);

        // Assert

        for (var i = 1; i < numFiles; i++)
        {
            var exists = await service.ExistsAsync(containerName, fileName + i);
            Assert.False(exists);
            exists = await service.ExistsAsync(containerName, directoryName + fileName + i);
            Assert.False(exists);
        }
    }

    [Fact]
    public async Task ClearContainer_IfEmpty_DoesNotThrowException()
    {
        var uniqueData = MakeContainerName(nameof(ClearContainer_IfEmpty_DoesNotThrowException));
        var containerName = uniqueData;

        var service = CreateFileStoreService();
        await service.ClearContainerAsync(containerName);

        Assert.True(true);
    }

    [Theory]
    [InlineData("sub", 1)]
    [InlineData("sub/", 2)]
    public async Task ClearDirectory_IfMultipleFile_DoesClear(string directoryName, int testIteration)
    {
        var uniqueData = MakeContainerName(nameof(ClearDirectory_IfMultipleFile_DoesClear) + testIteration);
        var containerName = uniqueData;
        var formattedDirectoryName = directoryName.Trim('/') + '/';
        var fileName = GetFileName(uniqueData);
        var numFiles = 5;

        var service = CreateFileStoreService();

        // Setup files
        for (var i = 1; i < numFiles; i++)
        {
            // in root
            using (var stream = GetFileStream(containerName))
            {
                await service.CreateAsync(containerName, fileName + i, stream);
            }

            // in sub directory
            using (var stream = GetFileStream(uniqueData))
            {
                await service.CreateAsync(containerName, formattedDirectoryName + fileName + i, stream);
            }
        }

        // Act

        await service.ClearDirectoryAsync(containerName, directoryName);

        // Assert

        for (var i = 1; i < numFiles; i++)
        {
            var exists = await service.ExistsAsync(containerName, fileName + i);
            Assert.True(exists);
            exists = await service.ExistsAsync(containerName, formattedDirectoryName + fileName + i);
            Assert.False(exists);
        }
    }

    [Fact]
    public async Task CreateAsync_IfNotExists_DoesNotThrow()
    {
        var uniqueData = MakeContainerName(nameof(CreateAsync_IfNotExists_DoesNotThrow));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();

        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
        }

        Assert.True(true);
    }

    [Fact]
    public async Task CreateAsync_IfExists_Throws()
    {
        var uniqueData = MakeContainerName(nameof(CreateAsync_IfExists_Throws));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
        }

        using (var stream = GetFileStream(uniqueData))
        {
            // TODO: catch specific exception
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
            {
                return service.CreateAsync(uniqueData, fileName, stream);
            });
        }
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_IfNotExists_Creates()
    {
        var uniqueData = MakeContainerName(nameof(CreateIfNotExistsAsync_IfNotExists_Creates));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();

        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateIfNotExistsAsync(uniqueData, fileName, stream);

            var exists = await service.ExistsAsync(uniqueData, fileName);
            Assert.True(exists);
        }
    }

    [Fact]
    public async Task CreateIfNotExistsAsync_IfExists_DoesNotCreate()
    {
        var uniqueData = MakeContainerName(nameof(CreateIfNotExistsAsync_IfExists_DoesNotCreate));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        var file1Text = uniqueData + "file1";
        byte[] file1Bytes;

        using (var stream = GetFileStream(file1Text))
        {
            file1Bytes = stream.ToArray();
            await service.CreateAsync(uniqueData, fileName, stream);
        }

        var file2Text = uniqueData + "file2";
        using (var stream = GetFileStream(file2Text))
        {
            await service.CreateIfNotExistsAsync(uniqueData, fileName, stream);
        }

        byte[] savedFileBytes;
        using var savedFileStream = await service.GetAsync(uniqueData, fileName);
        using var memoryStream = new MemoryStream();

        if (savedFileStream != null)
        {
            await savedFileStream.CopyToAsync(memoryStream);
        }
        savedFileBytes = memoryStream.ToArray();

        Assert.Equal(file1Bytes, savedFileBytes);
    }

    [Fact]
    public async Task CreateOrReplaceAsync_IfNotExists_Creates()
    {
        var uniqueData = MakeContainerName(nameof(CreateOrReplaceAsync_IfNotExists_Creates));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateOrReplaceAsync(uniqueData, fileName, stream);

            var exists = await service.ExistsAsync(uniqueData, fileName);
            Assert.True(exists);
        }
    }

    [Fact]
    public async Task CreateOrReplaceAsync_IfExists_Replaces()
    {
        var uniqueData = MakeContainerName(nameof(CreateOrReplaceAsync_IfExists_Replaces));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        var file1Text = uniqueData + "file1";

        using (var stream = GetFileStream(file1Text))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
        }

        byte[] file2Bytes;
        var file2Text = uniqueData + "file2";
        using (var stream = GetFileStream(file2Text))
        {
            file2Bytes = stream.ToArray();
            await service.CreateOrReplaceAsync(uniqueData, fileName, stream);
        }

        byte[] savedFileBytes;
        using var savedFileStream = await service.GetAsync(uniqueData, fileName);
        using var memoryStream = new MemoryStream();

        if (savedFileStream != null)
        {
            await savedFileStream.CopyToAsync(memoryStream);
        }
        savedFileBytes = memoryStream.ToArray();

        Assert.Equal(file2Bytes, savedFileBytes);
    }

    [Fact]
    public async Task DeleteAsync_IfNotExists_DoesNotThrow()
    {
        var uniqueData = MakeContainerName(nameof(DeleteAsync_IfNotExists_DoesNotThrow));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        await service.DeleteAsync(uniqueData, fileName);

        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_IfExists_Deletes()
    {
        var uniqueData = MakeContainerName(nameof(DeleteAsync_IfExists_Deletes));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
        }

        await service.DeleteAsync(uniqueData, fileName);

        var exists = await service.ExistsAsync(uniqueData, fileName);
        Assert.False(exists);
    }

    [Theory]
    [InlineData("sub", 1)]
    [InlineData("sub/", 2)]
    public async Task DeleteDirectory_IfMultipleFile_DoesClear(string directoryName, int testIteration)
    {
        var uniqueData = MakeContainerName(nameof(DeleteDirectory_IfMultipleFile_DoesClear) + testIteration);
        var containerName = uniqueData;
        var formattedDirectoryName = directoryName.Trim('/') + '/';
        var fileName = GetFileName(uniqueData);
        var numFiles = 5;

        var service = CreateFileStoreService();
        // Setup files
        for (var i = 1; i < numFiles; i++)
        {
            // in root
            using (var stream = GetFileStream(containerName))
            {
                await service.CreateAsync(containerName, fileName + i, stream);
            }

            // in sub directory
            using (var stream = GetFileStream(uniqueData))
            {
                await service.CreateAsync(containerName, formattedDirectoryName + fileName + i, stream);
            }
        }

        // Act

        await service.DeleteDirectoryAsync(containerName, directoryName);

        // Assert

        for (var i = 1; i < numFiles; i++)
        {
            var exists = await service.ExistsAsync(containerName, fileName + i);
            Assert.True(exists);
            exists = await service.ExistsAsync(containerName, formattedDirectoryName + fileName + i);
            Assert.False(exists);
        }
    }

    [Fact]
    public async Task ExistsAsync_IfNotExists_ReturnsFalse()
    {
        var uniqueData = MakeContainerName(nameof(ExistsAsync_IfNotExists_ReturnsFalse));
        var fileName = GetFileName(uniqueData);
        bool exists;

        var service = CreateFileStoreService();
        exists = await service.ExistsAsync(uniqueData, fileName);

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsAsync_IfExists_ReturnsTrue()
    {
        var uniqueData = MakeContainerName(nameof(ExistsAsync_IfExists_ReturnsTrue));
        var fileName = GetFileName(uniqueData);
        bool exists;

        var service = CreateFileStoreService();
        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
            exists = await service.ExistsAsync(uniqueData, fileName);
        }

        Assert.True(exists);
    }

    [Fact]
    public async Task GetAsync_IfExists_ReturnsFile()
    {
        var uniqueData = MakeContainerName(nameof(GetAsync_IfExists_ReturnsFile));
        var fileName = GetFileName(uniqueData);
        byte[] fileBytes;

        var service = CreateFileStoreService();
        using (var stream = GetFileStream(uniqueData))
        {
            await service.CreateAsync(uniqueData, fileName, stream);
            fileBytes = stream.ToArray();

            byte[] savedFileBytes;
            using var savedFileStream = await service.GetAsync(uniqueData, fileName);
            using var memoryStream = new MemoryStream();

            if (savedFileStream != null)
            {
                await savedFileStream.CopyToAsync(memoryStream);
            }
            savedFileBytes = memoryStream.ToArray();

            Assert.Equal(fileBytes, savedFileBytes);
        }
    }

    [Fact]
    public async Task GetAsync_IfNotExists_ReturnsNull()
    {
        var uniqueData = MakeContainerName(nameof(GetAsync_IfNotExists_ReturnsNull));
        var fileName = GetFileName(uniqueData);

        var service = CreateFileStoreService();
        using (var fileStream = await service.GetAsync(uniqueData, fileName))
        {
            Assert.Null(fileStream);
        }
    }

    private static string MakeContainerName(string uniqueData)
    {
        return SlugFormatter.ToSlug(uniqueData).Replace("-", "") + TEST_RUN_ID;
    }

    private AzureBlobFileService CreateFileStoreService()
    {
        return new AzureBlobFileService(_azureSettings);
    }

    private MemoryStream GetFileStream(string uniqueData)
    {
        var text = $"This is a file written by {nameof(AzureBlobFileService)}, unique data: {uniqueData}";
        var bytes = Encoding.UTF8.GetBytes(text);
        var stream = new MemoryStream(bytes);

        return stream;
    }

    private static string GetFileName(string uniqueData)
    {
        return Path.ChangeExtension(uniqueData, "txt");
    }
}
