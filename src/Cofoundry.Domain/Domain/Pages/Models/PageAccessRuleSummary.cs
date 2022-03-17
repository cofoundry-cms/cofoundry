namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// Access rules are used to restrict access to a website resource to users
/// fulfilling certain criteria such as a specific user area or role. Page
/// access rules are used to define the rules at a page level, 
/// however rules are also inherited from the directories the page is parented to.
/// </para>
/// <para>
/// Note that access rules do not apply to users from the Cofoundry Admin user
/// area. They aren't intended to be used to restrict editor access in the admin UI 
/// but instead are used to restrict public access to website pages and routes.
/// </para>
/// </summary>
/// <inheritdoc/>
public class PageAccessRuleSummary : IEntityAccessRuleSummary
{
    /// <summary>
    /// Database primary key.
    /// </summary>
    public int PageAccessRuleId { get; set; }

    /// <summary>
    /// Id of the page that this rule controls access to.
    /// </summary>
    public int PageId { get; set; }

    public UserAreaMicroSummary UserArea { get; set; }

    public RoleMicroSummary Role { get; set; }
}
