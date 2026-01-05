namespace Cofoundry.Domain.Data;

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
public class PageAccessRule : IEntityAccessRule
{
    /// <summary>
    /// Database primary key.
    /// </summary>
    public int PageAccessRuleId { get; set; }

    /// <summary>
    /// Id of the <see cref="Page"/> that this rule controls access to.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// <see cref="Page"/> that this rule controls access to.
    /// </summary>
    public Page Page
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageAccessRule>(nameof(Page));
        set;
    }

    /// <inheritdoc/>
    public string UserAreaCode { get; set; } = string.Empty;

    /// <inheritdoc/>
    public UserArea UserArea
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageAccessRule>(nameof(UserArea));
        set;
    }

    /// <inheritdoc/>
    public int? RoleId { get; set; }

    /// <inheritdoc/>
    public Role? Role { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageAccessRule>(nameof(Creator));
        set;
    }

    /// <inheritdoc/>
    public int GetId()
    {
        return PageAccessRuleId;
    }
}
