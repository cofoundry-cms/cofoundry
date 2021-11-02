using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageAccessRulesRepository
    {
        /// <summary>
        /// Updates all access rules associated with a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageAccessRulesCommand command);
    }
}
