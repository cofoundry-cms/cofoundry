namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// An indexable resource to be included in a sitemap.
/// </summary>
/// <remarks>
/// See http://www.sitemaps.org/en_GB/protocol.html
/// </remarks>
public interface ISiteMapResource
{
    string Url { get; set; }

    DateTime? LastModifiedDate { get; set; }

    decimal? Priority { get; set; }
}
