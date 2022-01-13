using System.Threading.Tasks;

namespace Cofoundry.Web.Internal
{
    /// <summary>
    /// Internal repository for retreiving the primary user data 
    /// fields required to build a claim principal.
    /// </summary>
    public interface IClaimsPrincipalBuilderContextRepository
    {
        /// <summary>
        /// Fetches user data for building a claims principal, returning
        /// <see langword="null"/> if the user cannot be found. This method
        /// bypasses authorization because it is used internally as part
        /// of user login and session management.
        /// </summary>
        /// <param name="userId">
        /// The identifier of the user to return data for.
        /// </param>
        Task<IClaimsPrincipalBuilderContext> GetAsync(int userId);
    }
}
