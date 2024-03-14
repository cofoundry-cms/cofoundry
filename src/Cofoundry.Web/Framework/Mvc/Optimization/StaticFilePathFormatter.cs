using Microsoft.Extensions.Caching.Memory;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IStaticFilePathFormatter"/>.
/// </summary>
public class StaticFilePathFormatter : IStaticFilePathFormatter
{
    private readonly IMemoryCache _memoryCache;
    private readonly IStaticResourceFileProvider _staticResourceFileProvider;

    public StaticFilePathFormatter(
        IMemoryCache memoryCache,
        IStaticResourceFileProvider staticResourceFileProvider
        )
    {
        _memoryCache = memoryCache;
        _staticResourceFileProvider = staticResourceFileProvider;
    }

    /// <inheritdoc/>
    public string AppendVersion(string applicationRelativePath)
    {
        // Only support site relative urls, so no need to pass requestPathBase;
        var versionProvider = new CofoundryFileVersionProvider(_staticResourceFileProvider, _memoryCache);
        var path = versionProvider.AddFileVersionToPath(null, applicationRelativePath);

        return path;
    }
}
