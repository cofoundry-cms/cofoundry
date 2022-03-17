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
/// <inheritdoc/>
public class PageDirectoryAccessRule : IEntityAccessRule
{
    /// <summary>
    /// Database primary key.
    /// </summary>
    public int PageDirectoryAccessRuleId { get; set; }

    /// <summary>
    /// Id of the <see cref="PageDirectory"/> that this rule controls access 
    /// to, as well as any child directories or pages.
    /// </summary>
    public int PageDirectoryId { get; set; }

    /// <summary>
    /// <see cref="PageDirectory"/> that this rule controls access to,
    /// as well as any child directories or pages.
    /// </summary>
    public virtual PageDirectory PageDirectory { get; set; }

    public string UserAreaCode { get; set; }

    public virtual UserArea UserArea { get; set; }

    public int? RoleId { get; set; }

    public virtual Role Role { get; set; }

    public DateTime CreateDate { get; set; }

    public int CreatorId { get; set; }

    public virtual User Creator { get; set; }

    public int GetId()
    {
        return PageDirectoryAccessRuleId;
    }
}
