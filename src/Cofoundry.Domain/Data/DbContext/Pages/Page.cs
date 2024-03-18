namespace Cofoundry.Domain.Data;

/// <summary>
/// Pages represent the dynamically navigable pages of your website. Each page uses a template 
/// which defines the regions of content that users can edit. Pages are a versioned entity and 
/// therefore have many page version records. At one time a page may only have one draft 
/// version, but can have many published versions; the latest published version is the one that 
/// is rendered when the page is published. 
/// </summary>
public class Page : IEntityAccessRestrictable<PageAccessRule>, ICreateAuditable, IEntityPublishable
{
    /// <summary>
    /// The auto-incrementing database id of the page.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// The id of the <see cref="PageDirectory"/> this page is in.
    /// </summary>
    public int PageDirectoryId { get; set; }

    private PageDirectory? _pageDirectory;
    /// <summary>
    /// The <see cref="Data.PageDirectory"/> this page is in.
    /// </summary>
    public PageDirectory PageDirectory
    {
        get => _pageDirectory ?? throw NavigationPropertyNotInitializedException.Create<Page>(nameof(PageDirectory));
        set => _pageDirectory = value;
    }

    /// <summary>
    /// Optional id of the <see cref="Locale"/> if used in a localized site.
    /// </summary>
    public int? LocaleId { get; set; }

    /// <summary>
    /// Optional <see cref="Locale"/> of the page if used in a localized site.
    /// </summary>
    public Locale? Locale { get; set; }

    /// <summary>
    /// The path of the page within the directory. This must be
    /// unique within the directory the page is parented to.
    /// </summary>
    public string UrlPath { get; set; } = string.Empty;

    /// <summary>
    /// Most pages are generic pages but they could have some sort of
    /// special function e.g. NotFound, CustomEntityDetails. This is the
    /// numeric representation of the domain <see cref="Cofoundry.Domain.PageType"/> 
    /// enum.
    /// </summary>
    public int PageTypeId { get; set; }

    /// <summary>
    /// If this is of <see cref="PageType.CustomEntityDetails"/>, this is used
    /// to look up the routing.
    /// </summary>
    public string? CustomEntityDefinitionCode { get; set; }

    /// <summary>
    /// If this is of <see cref="PageType.CustomEntityDetails"/>, this is used
    /// to look up the routing.
    /// </summary>
    public CustomEntityDefinition? CustomEntityDefinition { get; set; }

    /// <inheritdoc/>
    public string PublishStatusCode { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime? PublishDate { get; set; }

    /// <inheritdoc/>
    public DateTime? LastPublishDate { get; set; }

    /// <inheritdoc/>
    public int AccessRuleViolationActionId { get; set; }

    /// <inheritdoc/>
    public string? UserAreaCodeForSignInRedirect { get; set; }

    /// <inheritdoc/>
    public UserArea? UserAreaForSignInRedirect { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<Page>(nameof(Creator));
        set => _creator = value;
    }

    public ICollection<PageGroupItem> PageGroupItems { get; set; } = new List<PageGroupItem>();

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public ICollection<PageTag> PageTags { get; set; } = new List<PageTag>();

    /// <summary>
    /// Pages are a versioned entity and therefore have many page version
    /// records. At one time a page may only have one draft version, but
    /// can have many published versions; the latest published version is
    /// the one that is rendered when the page is published. 
    /// </summary>
    public ICollection<PageVersion> PageVersions { get; set; } = new List<PageVersion>();

    /// <summary>
    /// Lookup cache used for quickly finding the correct version for a
    /// specific publish status query e.g. 'Latest', 'Published', 'PreferPublished'
    /// </summary>
    public ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; } = new List<PagePublishStatusQuery>();

    /// <summary>
    /// <para>
    /// Access rules are used to restrict access to a website resource to users
    /// fulfilling certain criteria such as a specific user area or role. Page
    /// access rules are used to define the rules at a <see cref="Page"/> level, 
    /// however rules are also inherited from the directories the page is parented to.
    /// </para>
    /// <para>
    /// Note that access rules do not apply to users from the Cofoundry Admin user
    /// area. They aren't intended to be used to restrict editor access in the admin UI 
    /// but instead are used to restrict public access to website pages and routes.
    /// </para>
    /// </summary>
    public ICollection<PageAccessRule> AccessRules { get; set; } = new List<PageAccessRule>();

    /// <inheritdoc/>
    public int GetId()
    {
        return PageId;
    }
}
