using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageAccessRulesRepository
    {
        /// <summary>
        /// Determines if a page access rule already exists with the specified 
        /// rule configuration. 
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsUnique(IsPageAccessRuleUniqueQuery query);

        /// <summary>
        /// Adds a new access rule to a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created access rule.</returns>
        Task<int> AddAsync(AddPageAccessRuleCommand command);

        /// <summary>
        /// Updates an existing page access rule.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageAccessRuleCommand command);

        /// <summary>
        /// Removes an access rule from a page.
        /// </summary>
        /// <param name="pageAccessRuleId">Id of the access rule to delete.</param>
        Task DeleteAsync(int pageAccessRuleId);
    }
}
