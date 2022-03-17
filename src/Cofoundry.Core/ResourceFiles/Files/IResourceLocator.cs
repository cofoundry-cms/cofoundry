using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Core.ResourceFiles;

/// <summary>
/// Used to locate files, whether they be physical files on disk, embedded resources
/// or some other abstraction.
/// </summary>
public interface IResourceLocator
{
    /// <summary>
    /// True if the file exists; otherwise false
    /// </summary>
    bool FileExists(string virtualPath);

    /// <summary>
    /// Gets a references to the file if it exists, otherwise returns null
    /// </summary>
    IFileInfo GetFile(string virtualPath);

    /// <summary>
    /// True if the directory exists; otherwise false.
    /// </summary>
    bool DirectoryExists(string virtualDir);

    /// <summary>
    /// Gets a reference to the directory at the specified path if it exists, otherwise returns null
    /// </summary>
    IDirectoryContents GetDirectory(string virtualDir);
}