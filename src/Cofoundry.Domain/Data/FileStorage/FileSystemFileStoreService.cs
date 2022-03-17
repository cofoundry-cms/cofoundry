namespace Cofoundry.Domain.Data.Internal;

/// <summary>
/// File storage abstraction using the file system
/// </summary>
/// <inheritdoc/>
public class FileSystemFileStoreService : IFileStoreService
{
    private readonly Lazy<string> _fileRoot;

    private readonly FileSystemFileStorageSettings _fileSystemFileStorageSettings;
    private readonly IPathResolver _pathResolver;

    public FileSystemFileStoreService(
        FileSystemFileStorageSettings fileSystemFileStorageSettings,
        IPathResolver pathResolver
        )
    {
        _fileSystemFileStorageSettings = fileSystemFileStorageSettings;
        _pathResolver = pathResolver;
        _fileRoot = new Lazy<string>(SetFilePath);
    }

    private string SetFilePath()
    {
        string fileRoot = _pathResolver.MapPath(_fileSystemFileStorageSettings.FileRoot);

        if (!Directory.Exists(fileRoot))
        {
            Directory.CreateDirectory(fileRoot);
        }

        return fileRoot;
    }

    public Task<bool> ExistsAsync(string containerName, string fileName)
    {
        var exists = File.Exists(Path.Combine(_fileRoot.Value, containerName, fileName));
        return Task.FromResult(exists);
    }

    public Task<Stream> GetAsync(string containerName, string fileName)
    {
        var path = Path.Combine(_fileRoot.Value, containerName, fileName);
        Stream fileStream = null;

        if (File.Exists(path))
        {
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        return Task.FromResult(fileStream);
    }

    public Task CreateAsync(string containerName, string fileName, System.IO.Stream stream)
    {
        return CreateFileAsync(containerName, fileName, stream, FileMode.CreateNew);
    }

    public Task CreateOrReplaceAsync(string containerName, string fileName, Stream stream)
    {
        return CreateFileAsync(containerName, fileName, stream, FileMode.Create);
    }

    public Task CreateIfNotExistsAsync(string containerName, string fileName, System.IO.Stream stream)
    {
        var path = Path.Combine(_fileRoot.Value, containerName, fileName);
        if (File.Exists(path)) return Task.CompletedTask;

        return CreateFileAsync(containerName, fileName, stream, FileMode.CreateNew);
    }

    public Task DeleteAsync(string containerName, string fileName)
    {
        var path = Path.Combine(_fileRoot.Value, containerName, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public Task DeleteDirectoryAsync(string containerName, string directoryName)
    {
        var dir = Path.Combine(_fileRoot.Value, containerName, directoryName);
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }

        return Task.CompletedTask;
    }

    public Task ClearDirectoryAsync(string containerName, string directoryName)
    {
        var dir = Path.Combine(_fileRoot.Value, containerName, directoryName);
        if (Directory.Exists(dir))
        {
            var directory = new DirectoryInfo(dir);
            foreach (var file in directory.GetFiles())
            {
                if (file.IsReadOnly) file.IsReadOnly = false;
                file.Delete();
            }
            foreach (var childDirectory in directory.GetDirectories())
            {
                childDirectory.Delete(true);
            }
        }

        return Task.CompletedTask;
    }

    public Task ClearContainerAsync(string containerName)
    {
        return ClearDirectoryAsync(containerName, string.Empty);
    }

    private async Task CreateFileAsync(string containerName, string fileName, Stream stream, FileMode fileMode)
    {
        var path = Path.Combine(_fileRoot.Value, containerName, fileName);
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (var fs = new FileStream(path, fileMode))
        {
            stream.Position = 0;
            await stream.CopyToAsync(fs);
        }
    }
}
