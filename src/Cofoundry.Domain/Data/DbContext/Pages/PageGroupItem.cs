namespace Cofoundry.Domain.Data;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroupItem : ICreateAuditable
{
    public int PageId { get; set; }

    public Page Page
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(Page));
        set;
    }

    public int PageGroupId { get; set; }

    public PageGroup PageGroup
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(PageGroup));
        set;
    }

    public int Ordering { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageGroupItem>(nameof(Creator));
        set;
    }
}
