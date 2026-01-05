namespace Cofoundry.Domain.Data;

/// <summary>
/// <para>
/// Access rules are used to restrict access to a website resource to users
/// fulfilling certain criteria such as a specific user area or role. Page
/// directory access rules are used to define the rules at a <see cref="PageDirectory"/> 
/// level. These rules are inherited by child directories and pages.
/// </para>
/// <para>
/// Note that access rules do not apply to users from the Cofoundry Admin user
/// area. They aren't intended to be used to restrict editor access in the admin UI 
/// but instead are used to restrict public access to website pages and routes.
/// </para>
/// </summary>
public class PageDirectoryAccessRule : IEntityAccessRule
{
    /// <summary>
    /// Database primary key.
    /// </summary>
    public int PageDirectoryAccessRuleId { get; set; }

    /// <summary>
    /// Id of the <see cref="Data.PageDirectory"/> that this rule controls access 
    /// to, as well as any child directories or pages.
    /// </summary>
    public int PageDirectoryId { get; set; }

    /// <summary>
    /// <see cref="Data.PageDirectory"/> that this rule controls access to,
    /// as well as any child directories or pages.
    /// </summary>
    public PageDirectory PageDirectory
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryAccessRule>(nameof(PageDirectory));
        set;
    }

    /// <inheritdoc/>
    public string UserAreaCode { get; set; } = string.Empty;

    /// <inheritdoc/>
    public UserArea UserArea
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryAccessRule>(nameof(UserArea));
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
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryAccessRule>(nameof(Creator));
        set;
    }

    /// <inheritdoc/>
    public int GetId()
    {
        return PageDirectoryAccessRuleId;
    }
}
