using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Core;

/// <summary>
/// This factory allows us to get hold of an IFileProvider that can be
/// configured after the container has been configured.
/// </summary>
public interface IResourceFileProviderFactory
{
    /// <summary>
    /// Creates a new IFileProvider instances that represents
    /// all locations in the application that can contain application 
    /// resources such as view files.
    /// </summary>
    IFileProvider Create();
}
