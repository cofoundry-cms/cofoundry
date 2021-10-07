using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryAccessRulesRepository
    {
        /// <summary>
        /// Determines if a page directory  access rule already exists with the specified 
        /// rule configuration. 
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsUnique(IsPageDirectoryAccessRuleUniqueQuery query);

        /// <summary>
        /// Adds a new access rule to a page directory.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created access rule.</returns>
        Task<int> AddAsync(AddPageDirectoryAccessRuleCommand command);

        /// <summary>
        /// Updates an existing page directory access rule.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageDirectoryAccessRuleCommand command);

        /// <summary>
        /// Removes an access rule from a directory.
        /// </summary>
        /// <param name="pageDirectoryAccessRuleId">Id of the access rule to delete.</param>
        Task DeleteAsync(int pageDirectoryAccessRuleId);
    }
}
