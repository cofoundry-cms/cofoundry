using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Cofoundry.Core.Configuration;

namespace Cofoundry.DocGenerator;

public class AzureBlobDestinationFileStoreService : IDestinationFileStoreService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "docs";
    private bool _isInitialized;

    public AzureBlobDestinationFileStoreService(DocGeneratorSettings docGeneratorSettings)
    {
        if (string.IsNullOrWhiteSpace(docGeneratorSettings.BlobStorageConnectionString))
        {
            throw new InvalidConfigurationException(typeof(DocGeneratorSettings), "The BlobStorageConnectionString is required when writing docs to azure.");
        }

        _blobServiceClient = new BlobServiceClient(docGeneratorSettings.BlobStorageConnectionString);
    }

    public async Task ClearDirectoryAsync(string folderName)
    {
        folderName = FormatBlobFilePath(folderName);
        var container = await GetBlobContainerAsync();

        var blobs = new List<BlobItem>();

        await foreach (var item in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, folderName))
        {
            blobs.Add(item);
        }

        await DeleteBlobsAsync(container, blobs);
    }

    public async Task<string[]> GetDirectoryNamesAsync(string path)
    {
        path = FormatBlobFilePath(path);
        var container = await GetBlobContainerAsync();

        var directories = new List<BlobHierarchyItem>();

        await foreach (var item in container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, path))
        {
            if (item.IsPrefix)
            {
                directories.Add(item);
            }
        }

        var directoryNames = directories
            .Select(d => d.Prefix.Trim('/'))
            .ToArray();

        return directoryNames;
    }

    public async Task CopyFile(string source, string destination)
    {
        destination = FormatBlobFilePath(destination);
        var container = await GetBlobContainerAsync();

        using (var stream = File.OpenRead(source))
        {
            var blockClient = container.GetBlobClient(destination);
            await blockClient.UploadAsync(stream, true);
        }
    }

    public async Task WriteText(string text, string destination)
    {
        destination = FormatBlobFilePath(destination);

        using (var stream = new MemoryStream())
        using (var writer = new StreamWriter(stream, Encoding.UTF8))
        {
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            var container = await GetBlobContainerAsync();

            var blockClient = container.GetBlobClient(destination);
            await blockClient.UploadAsync(stream, true);
        }
    }

    public Task EnsureDirectoryExistsAsync(string relativePath)
    {
        // no concept of empty directories in azure blog service
        return Task.CompletedTask;
    }

    private static string FormatBlobFilePath(string path)
    {
        return path.TrimStart('/');
    }

    private static async Task DeleteBlobsAsync(BlobContainerClient container, IEnumerable<BlobItem> blobs)
    {
        foreach (var blobItem in blobs)
        {
            await container.DeleteBlobIfExistsAsync(blobItem.Name, DeleteSnapshotsOption.IncludeSnapshots);
        }
    }

    private async Task<BlobContainerClient> GetBlobContainerAsync()
    {
        var containerName = _containerName.ToLower();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);

        // initalize container
        if (!_isInitialized)
        {
            await container.CreateIfNotExistsAsync();
            _isInitialized = true;
        }

        return container;
    }
}
