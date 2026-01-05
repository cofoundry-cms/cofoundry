namespace Cofoundry.Domain.Data;

public class PageTag : IEntityTag, ICreateAuditable
{
    public int PageId { get; set; }

    public Page Page
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Page));
        set;
    }

    public int TagId { get; set; }

    public Tag Tag
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Tag));
        set;
    }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Creator));
        set;
    }
}
