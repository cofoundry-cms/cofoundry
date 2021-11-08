using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Access rules for a single page directory that is inherited by another page or directory.
    /// </summary>
    public class InheritedPageDirectoryAccessDetails : IEntityAccessRuleSetDetails<PageDirectoryAccessRuleSummary>
    {
        /// <summary>
        /// Database id of the page these access rules are associated with.
        /// </summary>
        public PageDirectoryMicroSummary PageDirectory { get; set; }

        /// <summary>
        /// <para>
        /// Access rules are used to restrict access to a website resource to users
        /// fulfilling certain criteria such as a specific user area or role. Page
        /// directory access rules are used to define the rules at a page directory 
        /// level. These rules are inherited by child directories and pages.
        /// </para>
        /// <para>
        /// Note that access rules do not apply to users from the Cofoundry Admin user
        /// area. They aren't intended to be used to restrict editor access in the admin UI 
        /// but instead are used to restrict public access to website pages and routes.
        /// </para>
        /// </summary>
        public ICollection<PageDirectoryAccessRuleSummary> AccessRules { get; set; }

        public AccessRuleViolationAction ViolationAction { get; set; }

        public UserAreaMicroSummary UserAreaForLoginRedirect { get; set; }
    }
}
