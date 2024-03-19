namespace Cofoundry.Domain;

/// <summary>
/// Page data required to render a page, including template data for all the content-editable
/// regions. This object is specific to a particular version which may not always be the 
/// latest (depending on the query).
/// </summary>
public class PageRenderDetails
{
    /// <summary>
    /// The database id of the page.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// The database id of the page version this instance has been mapped 
    /// to. The version which may not always be the latest (depending on the query).
    /// </summary>
    public int PageVersionId { get; set; }

    /// <summary>
    /// The descriptive human-readable title of the page that is often 
    /// used in the html page title meta tag. Does not have to be
    /// unique.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the content of the page. This is intended to
    /// be used in the description html meta tag.
    /// </summary>
    public string? MetaDescription { get; set; }

    /// <summary>
    /// WorkFlowStatus of the version that this instance represents. The version
    /// may not always be the latest version and is dependent on the query that
    /// was used to load this instance, typically using a PublishStatusQuery value.
    /// </summary>
    public WorkFlowStatus WorkFlowStatus { get; set; }

    public OpenGraphData OpenGraph { get; set; } = new OpenGraphData();

    /// <summary>
    /// The routing data for the page.
    /// </summary>
    public PageRoute PageRoute { get; set; } = PageRoute.Uninitialized;

    /// <summary>
    /// The date the custom entity was created.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// The template used to render this page.
    /// </summary>
    public PageTemplateMicroSummary Template { get; set; } = PageTemplateMicroSummary.Uninitialized;

    /// <summary>
    /// Content-editable page region and block data for rendering out to the template.
    /// </summary>
    public IReadOnlyCollection<PageRegionRenderDetails> Regions { get; set; } = Array.Empty<PageRegionRenderDetails>();

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly PageRenderDetails Uninitialized = new()
    {
        PageId = int.MinValue
    };
}
