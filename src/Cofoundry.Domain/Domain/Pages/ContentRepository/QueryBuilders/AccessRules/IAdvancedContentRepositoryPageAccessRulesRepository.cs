using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageAccessRulesRepository
    {
        /// <summary>
        /// Retrieve page access configuration data by the id of the parent page.
        /// </summary>
        /// <param name="pageId">PageId of the page to get access configuration data for.</param>
        IAdvancedContentRepositoryPageAccessByPageIdQueryBuilder GetByPageId(int pageId);

        /// <summary>
        /// Updates all access rules associated with a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageAccessRuleSetCommand command);
    }
}
