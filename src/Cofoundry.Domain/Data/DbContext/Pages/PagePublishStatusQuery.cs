namespace Cofoundry.Domain.Data;

/// <summary>
/// Lookup cache used for quickly finding the correct version for a
/// specific publish status query e.g. 'Latest', 'Published', 
/// 'PreferPublished'. These records are generated when pages
/// are published or unpublished.
/// </summary>
public class PagePublishStatusQuery
{
    /// <summary>
    /// Id of the page this record represents. Forms a key
    /// with the PublishStatusQueryId.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// Numeric representation of the domain PublishStatusQuery enum.
    /// </summary>
    public short PublishStatusQueryId { get; set; }

    /// <summary>
    /// The id of the version of the page that should be displayed
    /// for the corresponding PublishStatusQueryId.
    /// </summary>
    public int PageVersionId { get; set; }

    private Page? _page;
    /// <summary>
    /// Page that this record represents.
    /// </summary>
    public Page Page
    {
        get => _page ?? throw NavigationPropertyNotInitializedException.Create<PagePublishStatusQuery>(nameof(Page));
        set => _page = value;
    }

    private PageVersion? _pageVersion;
    /// <summary>
    /// The version of the page that should be displayed
    /// for the corresponding PublishStatusQueryId.
    /// </summary>
    public PageVersion PageVersion
    {
        get => _pageVersion ?? throw NavigationPropertyNotInitializedException.Create<PagePublishStatusQuery>(nameof(PageVersion));
        set => _pageVersion = value;
    }

}
