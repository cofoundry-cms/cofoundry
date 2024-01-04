namespace Cofoundry.Domain.Data;

/// <summary>
/// Pages are a versioned entity and therefore have many page version
/// records. At one time a page may only have one draft version, but
/// can have many published versions; the latest published version is
/// the one that is rendered when the page is published. 
/// </summary>
public class PageVersion : ICreateAuditable, IEntityVersion
{
    /// <summary>
    /// Auto-incrementing primary key of the page version record 
    /// in the database.
    /// </summary>
    public int PageVersionId { get; set; }

    /// <summary>
    /// Id of the page this version is parented to.
    /// </summary>
    public int PageId { get; set; }

    private Page? _page;
    /// <summary>
    /// The page this version is parented to.
    /// </summary>
    public Page Page
    {
        get => _page ?? throw NavigationPropertyNotInitializedException.Create<PageVersion>(nameof(Page));
        set => _page = value;
    }

    /// <summary>
    /// Id of the template used to render this version.
    /// </summary>
    public int PageTemplateId { get; set; }

    private PageTemplate? _pageTemplate;
    /// <summary>
    /// The template used to render this version.
    /// </summary>
    public PageTemplate PageTemplate
    {
        get => _pageTemplate ?? throw NavigationPropertyNotInitializedException.Create<PageVersion>(nameof(PageTemplate));
        set => _pageTemplate = value;
    }

    /// <summary>
    /// A display-friendly version number that indicates
    /// it's position in the hisotry of all verions of a specific
    /// page. E.g. the first version for a page is version 1 and 
    /// the 2nd is version 2. The display version is unique per
    /// page.
    /// </summary>
    public int DisplayVersion { get; set; }

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

    /// <inheritdoc/>
    public int WorkFlowStatusId { get; set; }

    /// <summary>
    /// Indicates whether the page should show in the auto-generated site map
    /// that gets presented to search engine robots.
    /// </summary>
    public bool ExcludeFromSitemap { get; set; }

    /// <summary>
    /// A title that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    public string? OpenGraphTitle { get; set; }

    /// <summary>
    /// A description that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    public string? OpenGraphDescription { get; set; }

    /// <summary>
    /// An image that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    public int? OpenGraphImageId { get; set; }

    /// <summary>
    /// An image that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    public ImageAsset? OpenGraphImageAsset { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageVersion>(nameof(Creator));
        set => _creator = value;
    }

    /// <summary>
    /// Lookup cache used for quickly finding the correct version for a
    /// specific publish status query e.g. 'Latest', 'Published', 
    /// 'PreferPublished'.
    /// </summary>
    public ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; } = new List<PagePublishStatusQuery>();

    /// <summary>
    /// Page content data to be rendered in the page template.
    /// </summary>
    public ICollection<PageVersionBlock> PageVersionBlocks { get; set; } = new List<PageVersionBlock>();
}
