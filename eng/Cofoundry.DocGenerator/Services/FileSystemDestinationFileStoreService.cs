using System.Text;

namespace Cofoundry.DocGenerator;

/// <remarks>
/// This service is just for local testing and so doesn't 
/// need to be particularly robust.
/// </remarks>
public class FileSystemDestinationFileStoreService : IDestinationFileStoreService
{
    private readonly string _destinationBasePath;

    public FileSystemDestinationFileStoreService(string destinationBasePath)
    {
        _destinationBasePath = destinationBasePath;
    }

    public Task ClearDirectoryAsync(string folderName)
    {
        var fullPath = MapDestinationVirtualPath(folderName);
        Directory.Delete(fullPath, true);
        Directory.CreateDirectory(fullPath);

        return Task.CompletedTask;
    }

    public Task<string[]> GetDirectoryNamesAsync(string path)
    {
        var fullPath = MapDestinationVirtualPath(path);
        var directories = Directory.GetDirectories(fullPath);

        return Task.FromResult(directories);
    }

    public Task CopyFile(string source, string destination)
    {
        var fullDestinationPath = MapDestinationVirtualPath(destination);

        File.Copy(source, fullDestinationPath, true);

        return Task.CompletedTask;
    }

    public Task WriteText(string text, string destination)
    {
        var fullDestinationPath = MapDestinationVirtualPath(destination);

        File.WriteAllText(fullDestinationPath, text, Encoding.UTF8);

        return Task.CompletedTask;
    }

    public Task EnsureDirectoryExistsAsync(string relativePath)
    {
        var fullPath = MapDestinationVirtualPath(relativePath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        return Task.CompletedTask;
    }

    private string MapDestinationVirtualPath(string path)
    {
        var formattedPath = path.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        return Path.Combine(_destinationBasePath, formattedPath);
    }
}
