namespace Cofoundry.Domain.Data;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroupItem : ICreateAuditable
{
    public int PageId { get; set; }

    private Page? _page;
    public Page Page
    {
        get => _page ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(Page));
        set => _page = value;
    }

    public int PageGroupId { get; set; }

    private PageGroup? _pageGroup;
    public PageGroup PageGroup
    {
        get => _pageGroup ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(PageGroup));
        set => _pageGroup = value;
    }

    public int Ordering { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(Creator));
        set => _creator = value;
    }
}
