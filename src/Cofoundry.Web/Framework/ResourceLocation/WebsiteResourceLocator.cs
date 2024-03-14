using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Options;

namespace Cofoundry.Web;

/// <summary>
/// <see cref="IResourceLocator"/> wrapper around the file providers assigned to
/// the razor view engine.
/// </summary>
public class WebsiteResourceLocator : IResourceLocator
{
    private readonly IList<IFileProvider> _fileProviders;

    /// <inheritdoc/>
    public WebsiteResourceLocator(
        IOptions<MvcRazorRuntimeCompilationOptions> mvcRazorRuntimeCompilationOptions
        )
    {
        _fileProviders = mvcRazorRuntimeCompilationOptions.Value.FileProviders;
    }

    /// <inheritdoc/>
    public bool DirectoryExists(string virtualDir)
    {
        foreach (var fileProvider in _fileProviders)
        {
            if (fileProvider.GetDirectoryContents(virtualDir).Exists)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public bool FileExists(string virtualPath)
    {
        foreach (var fileProvider in _fileProviders)
        {
            if (fileProvider.GetFileInfo(virtualPath).Exists)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public IDirectoryContents GetDirectory(string virtualDir)
    {
        var directories = _fileProviders
            .Where(d => d.GetDirectoryContents(virtualDir).Exists)
            .ToList();

        // The directory might appear in multiple file providers, but
        // each may contain different files, so we need to return all matches
        if (directories.Count != 0)
        {
            return new CompositeDirectoryContents(directories, virtualDir);
        }

        return new NotFoundDirectoryContents();
    }

    /// <inheritdoc/>
    public IFileInfo GetFile(string virtualPath)
    {
        IFileInfo? file;

        foreach (var fileProvider in _fileProviders)
        {
            file = fileProvider.GetFileInfo(virtualPath);
            if (file.Exists)
            {
                return file;
            }
        }

        return new NotFoundFileInfo(virtualPath);
    }
}
