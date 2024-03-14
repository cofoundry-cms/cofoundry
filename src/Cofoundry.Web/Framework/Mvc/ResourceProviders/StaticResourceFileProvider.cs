using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IStaticResourceFileProvider"/>.
/// </summary>
public class StaticResourceFileProvider : IStaticResourceFileProvider
{
    private readonly IFileProvider _compositeFileProvider;

    public StaticResourceFileProvider(
        IFileProvider compositeFileProvider
        )
    {
        _compositeFileProvider = compositeFileProvider;
    }

    /// <inheritdoc/>
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _compositeFileProvider.GetDirectoryContents(subpath);
    }

    /// <inheritdoc/>
    public IFileInfo GetFileInfo(string subpath)
    {
        var fileInfo = _compositeFileProvider.GetFileInfo(subpath);
        return fileInfo;
    }

    /// <inheritdoc/>
    public IChangeToken Watch(string filter)
    {
        return _compositeFileProvider.Watch(filter);
    }
}
