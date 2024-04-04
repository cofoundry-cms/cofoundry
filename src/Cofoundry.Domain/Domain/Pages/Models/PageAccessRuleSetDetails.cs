namespace Cofoundry.Domain;

/// <summary>
/// Collated information about the access rules for a page including both those
/// directly associated and those inherited from parent directories.
/// </summary>
public class PageAccessRuleSetDetails : IEntityAccessRuleSetDetails<PageAccessRuleSummary>
{
    /// <summary>
    /// Database id of the page these access rules are associated with.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// Rules inherited from the directories this page is parented to.
    /// </summary>
    public IReadOnlyCollection<InheritedPageDirectoryAccessDetails> InheritedAccessRules { get; set; } = Array.Empty<InheritedPageDirectoryAccessDetails>();

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
    public IReadOnlyCollection<PageAccessRuleSummary> AccessRules { get; set; } = Array.Empty<PageAccessRuleSummary>();

    /// <inheritdoc/>
    public AccessRuleViolationAction ViolationAction { get; set; }

    /// <inheritdoc/>
    public UserAreaMicroSummary? UserAreaForSignInRedirect { get; set; }
}
