using Cofoundry.Core.Web;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IStaticFileViewHelper"/>.
/// </summary>
public class StaticFileViewHelper : IStaticFileViewHelper
{
    private readonly IStaticFilePathFormatter _staticFilePathFormatter;
    private readonly ISiteUrlResolver _siteUriResolver;

    public StaticFileViewHelper(
        IStaticFilePathFormatter staticFilePathFormatter,
        ISiteUrlResolver siteUriResolver
        )
    {
        _staticFilePathFormatter = staticFilePathFormatter;
        _siteUriResolver = siteUriResolver;
    }

    /// <inheritdoc/>
    public string AppendVersion(string applicationRelativePath)
    {
        var appended = _staticFilePathFormatter.AppendVersion(applicationRelativePath);
        return appended;
    }

    /// <inheritdoc/>
    public string ToAbsoluteWithFileVersion(string applicationRelativePath)
    {
        if (string.IsNullOrWhiteSpace(applicationRelativePath)) return applicationRelativePath;
        var appenedUrl = AppendVersion(applicationRelativePath);
        var resolvedUrl = _siteUriResolver.MakeAbsolute(appenedUrl);

        return resolvedUrl;
    }
}
