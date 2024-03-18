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

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageGroup>(nameof(Creator));
        set => _creator = value;
    }

    public ICollection<PageGroupItem> PageGroupItems { get; set; } = new List<PageGroupItem>();

    public ICollection<PageGroup> ChildPageGroups { get; set; } = new List<PageGroup>();
}
