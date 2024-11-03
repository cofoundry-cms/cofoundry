using System.Collections.Concurrent;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Cofoundry.Core.Configuration;
using Cofoundry.Domain.Data;

namespace Cofoundry.Plugins.Azure.Internal;

/// <summary>
/// File system abstraction for Azure blob storage
/// </summary>
public class AzureBlobFileService : IFileStoreService
{
    private readonly BlobServiceClient _blobServiceClient;
    private static readonly ConcurrentDictionary<string, byte> _initializedContainers = new();

    public AzureBlobFileService(
        AzureSettings settings
        )
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(settings.BlobStorageConnectionString))
        {
            throw new InvalidConfigurationException(typeof(AzureSettings), $"The {nameof(settings.BlobStorageConnectionString)} setting is required to use the {nameof(AzureBlobFileService)}");
        }

        _blobServiceClient = new BlobServiceClient(settings.BlobStorageConnectionString);
    }

    public BlobServiceClient GetServiceClient()
    {
        return _blobServiceClient;
    }

    /// <summary>
    /// Determins if the specified file exists in the container.
    /// </summary>
    /// <param name="containerName">The name of the container to look for the file.</param>
    /// <param name="fileName">Name of the file to look for.</param>
    /// <returns>True if the file exists; otherwise false.</returns>
    public async Task<bool> ExistsAsync(string containerName, string fileName)
    {
        var container = await GetBlobContainerAsync(containerName);
        var blobClient = container.GetBlobClient(fileName);

        return await blobClient.ExistsAsync();
    }

    /// <summary>
    /// Gets the specified file as a Stream. 
    /// </summary>
    /// <param name="containerName">The name of the container to look for the file</param>
    /// <param name="fileName">The name of the file to get</param>
    /// <returns>Stream reference to the file.</returns>
    public async Task<Stream?> GetAsync(string containerName, string fileName)
    {
        var container = await GetBlobContainerAsync(containerName);
        var blobClient = container.GetBlobClient(fileName);

        try
        {
            return await blobClient.OpenReadAsync();
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a new file, throwing an exception if a file already exists with the same filename
    /// </summary>
    public Task CreateAsync(string containerName, string fileName, Stream stream)
    {
        return CreateAsync(containerName, fileName, stream, true);
    }

    /// <summary>
    /// Saves a file, creating a new file or overwriting a file if it already exists.
    /// </summary>
    public async Task CreateOrReplaceAsync(string containerName, string fileName, Stream stream)
    {
        var container = await GetBlobContainerAsync(containerName);
        var blockClient = container.GetBlobClient(fileName);

        if (stream.Position != 0)
        {
            stream.Position = 0;
        }
        await blockClient.UploadAsync(stream, true);
    }

    /// <summary>
    /// Creates a new file if it doesn't exist already, otherwise the existing file is left in place.
    /// </summary>
    public Task CreateIfNotExistsAsync(string containerName, string fileName, Stream stream)
    {
        return CreateAsync(containerName, fileName, stream, false);
    }

    /// <summary>
    /// Deletes a file from the container if it exists.
    /// </summary>
    /// <param name="containerName">The name of the container containing the file to delete</param>
    /// <param name="fileName">Name of the file to delete</param>
    public async Task DeleteAsync(string containerName, string fileName)
    {
        var container = await GetBlobContainerAsync(containerName);
        var blockBlob = container.GetBlobClient(fileName);

        await blockBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }

    /// <summary>
    /// Deletes a directory including all files and sub-directories
    /// </summary>
    /// <param name="containerName">The name of the container containing the directory to delete</param>
    /// <param name="directoryName">The name of the directory to delete</param>
    public async Task DeleteDirectoryAsync(string containerName, string directoryName)
    {
        var container = await GetBlobContainerAsync(containerName);

        var blobs = new List<BlobItem>();

        await foreach (var item in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, directoryName))
        {
            blobs.Add(item);
        }

        await DeleteBlobsAsync(container, blobs);
    }

    /// <summary>
    /// Clears a directory deleting all files and sub-directories but not the directory itself
    /// </summary>
    /// <param name="containerName">The name of the container containing the directory to delete</param>
    /// <param name="directoryName">The name of the directory to delete</param>
    public Task ClearDirectoryAsync(string containerName, string directoryName)
    {
        return DeleteDirectoryAsync(containerName, directoryName);
    }

    /// <summary>
    /// Deletes all files in the container
    /// </summary>
    /// <param name="containerName">Name of the container to clear.</param>
    public async Task ClearContainerAsync(string containerName)
    {
        var container = await GetBlobContainerAsync(containerName);

        var blobs = new List<BlobItem>();

        await foreach (var item in container.GetBlobsAsync())
        {
            blobs.Add(item);
        }

        await DeleteBlobsAsync(container, blobs);
    }

    private static async Task DeleteBlobsAsync(BlobContainerClient container, IEnumerable<BlobItem> blobs)
    {
        foreach (var blobItem in blobs)
        {
            await container.DeleteBlobIfExistsAsync(blobItem.Name, DeleteSnapshotsOption.IncludeSnapshots);
        }
    }

    private async Task CreateAsync(string containerName, string fileName, Stream stream, bool throwExceptionIfNotExists)
    {
        var container = await GetBlobContainerAsync(containerName);
        var blobClient = container.GetBlobClient(fileName);

        // Don't overwrite:
        // http://stackoverflow.com/a/14938608/716689
        //var accessCondition = AccessCondition.GenerateIfNotExistsCondition();

        try
        {
            if (stream.Position != 0)
            {
                stream.Position = 0;
            }

            await blobClient.UploadAsync(stream);
            //await blobClient.UploadFromStreamAsync(stream, accessCondition, null, null);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            if (throwExceptionIfNotExists)
            {
                throw new InvalidOperationException("File already exists", ex);
            }
        }
    }

    private async Task<BlobContainerClient> GetBlobContainerAsync(string containerName)
    {
        containerName = containerName.ToLower();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);

        // initalize container
        if (_initializedContainers.TryAdd(containerName, 0))
        {
            await container.CreateIfNotExistsAsync();
        }

        return container;
    }
}
