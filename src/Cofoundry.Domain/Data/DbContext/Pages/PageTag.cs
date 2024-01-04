namespace Cofoundry.Domain.Data;

public class PageTag : IEntityTag, ICreateAuditable
{
    public int PageId { get; set; }

    private Page? _page;
    public Page Page
    {
        get => _page ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Page));
        set => _page = value;
    }

    public int TagId { get; set; }

    private Tag? _tag;
    public Tag Tag
    {
        get => _tag ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Tag));
        set => _tag = value;
    }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageTag>(nameof(Creator));
        set => _creator = value;
    }
}
