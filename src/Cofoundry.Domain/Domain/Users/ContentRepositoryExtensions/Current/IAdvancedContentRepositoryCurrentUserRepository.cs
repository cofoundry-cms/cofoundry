using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands relating to the currently logged in user.
    /// </summary>
    public interface IAdvancedContentRepositoryCurrentUserRepository : IContentRepositoryCurrentUserRepository
    {
        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdateCurrentUserCommand command);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// <see cref="UpdateCurrentUserPasswordCommand.OldPassword"/> field 
        /// to authenticate the request.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdatePasswordAsync(UpdateCurrentUserPasswordCommand command);
    }
}
