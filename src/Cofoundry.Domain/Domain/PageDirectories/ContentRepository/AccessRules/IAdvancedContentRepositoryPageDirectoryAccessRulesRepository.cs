using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryAccessRulesRepository
    {
        /// <summary>
        /// Updates all access rules associated with a page directory.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageDirectoryAccessRuleSetCommand command);
    }
}
