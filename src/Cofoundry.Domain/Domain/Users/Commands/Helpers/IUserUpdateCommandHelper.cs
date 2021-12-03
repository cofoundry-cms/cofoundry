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

        /// <summary>
        /// Updates the email properties (<see cref="User.Email"/>, 
        /// <see cref="User.UniqueEmail"/>, <see cref="User.EmailDomainId"/>)  
        /// on a user, handing normalization, uniquification and validation. If 
        /// the user area settings indicate that the email address is used for 
        /// the username, then these fields are also updated.
        /// </summary>
        /// <param name="userArea">The user area relating to the user being updated.</param>
        /// <param name="email">The unformattted email command property to update.</param>
        /// <param name="username">
        /// The username command property, or <see langword="null"/> if the command doesn't 
        /// have one. This is only required to validate it's been set correctly.
        /// </param>
        /// <param name="user">The new or existing user database record to update.</param>
        /// <param name="executionContext">
        /// The command execution context to pass down to any nested queries or commands.
        /// </param>
        Task UpdateEmailAsync(
            IUserAreaDefinition userArea,
            string email,
            string username,
            User user,
            IExecutionContext executionContext
            );
    }
}
