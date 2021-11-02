using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Collated information about the access rules for a page directory including both those
    /// directly associated and those inherited from parent directories.
    /// </summary>
    /// <inheritdoc/>
    public class PageDirectoryAccessInfo : InheritedPageDirectoryAccessInfo
    {
        /// <summary>
        /// Rules inherited from the directories this page is parented to.
        /// </summary>
        public ICollection<InheritedPageDirectoryAccessInfo> InheritedAccessRules { get; set; }
    }
}
