using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Helper for adding or updating the email and username
    /// property sets on a <see cref="User"/> record.
    /// </summary>
    public interface IUserUpdateCommandHelper
    {
        /// <summary>
        /// Updates the email and username properties (<see cref="User.Email"/>, 
        /// <see cref="User.UniqueEmail"/>, <see cref="User.EmailDomainId"/>, 
        /// <see cref="User.Username"/> and <see cref="User.UniqueUsername"/>)  
        /// on a user, handing normalization, uniquification and validation.
        /// </summary>
        /// <param name="email">The unformattted email command property to update.</param>
        /// <param name="username">The unformattted username command property to update.</param>
        /// <param name="user">The new or existing user database record to update.</param>
        /// <param name="executionContext">
        /// The command execution context to pass down to any nested queries or commands.
        /// </param>
        Task UpdateEmailAndUsernameAsync(
            string email,
            string username,
            User user,
            IExecutionContext executionContext
            );
    }
}
