namespace Cofoundry.Domain.Data;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroup : ICreateAuditable
{
    public int PageGroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public int? ParentGroupId { get; set; }

    public PageGroup? ParentPageGroup { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageGroup>(nameof(Creator));
        set;
    }

    public ICollection<PageGroupItem> PageGroupItems { get; set; } = new List<PageGroupItem>();

    public ICollection<PageGroup> ChildPageGroups { get; set; } = new List<PageGroup>();
}
