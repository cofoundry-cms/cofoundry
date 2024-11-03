using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// An indexable resource to be included in a sitemap.
/// </summary>
/// <remarks>
/// See http://www.sitemaps.org/en_GB/protocol.html
/// </remarks>
public class SiteMapResource : ISiteMapResource
{
    public SiteMapResource() { }

    public SiteMapResource(string url)
    {
        Url = url;
    }

    public SiteMapResource(string url, DateTime lastModifiedDate)
        : this(url)
    {
        LastModifiedDate = lastModifiedDate;
    }

    public SiteMapResource(string url, DateTime lastModifiedDate, decimal priority)
        : this(url, lastModifiedDate)
    {
        Priority = priority;
    }

    /// <summary>
    /// URL of the Resource. Can be relative.
    /// </summary>
    [MaxLength(2048)]
    [Required]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The date of last modification of the file. 
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }

    /// <summary>
    /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0. This value 
    /// has no effect on your pages compared to pages on other sites, and only lets the search engines know which of 
    /// your pages you deem most important so they can order the crawl of your pages in the way you would most like. 
    /// The default priority of a page is 0.5.
    /// </summary>
    /// <remarks>
    /// http://gsitecrawler.com/en/faq/info/priority/
    /// </remarks>
    [Range(0, 1)]
    public decimal? Priority { get; set; }
}
