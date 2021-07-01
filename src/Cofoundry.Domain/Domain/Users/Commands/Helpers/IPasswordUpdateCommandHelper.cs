using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by in password update commands for shared functionality.
    /// </summary>
    public interface IPasswordUpdateCommandHelper
    {
        /// <summary>
        /// Validates that the current user area has a password-based
        /// login and therefore accepts password updates.
        /// </summary>
        /// <param name="userArea">The user area to check.</param>
        void ValidateUserArea(IUserAreaDefinition userArea);

        /// <summary>
        /// Validates that the user executing the command has permission to update the
        /// users password.
        /// </summary>
        /// <param name="userArea">The user area of the user updating their password.</param>
        /// <param name="executionContext">The context of the command being executed.</param>
        void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext);

        /// <summary>
        /// Updates (but does not save) the password hash on a user record. Use this when
        /// updating the hash only i.e. when a hash upgrade is required.
        /// </summary>
        /// <param name="password">The password to re-hash and update the user record with.</param>
        /// <param name="user">The user record to update (but not save).</param>
        void UpdatePasswordHash(string password, User user);

        /// <summary>
        /// Updates the password on the user record, updating the RequirePasswordChange and
        /// LastPasswordChangeDate settings. Use this when you are updating a password based
        /// on user action.
        /// </summary>
        /// <param name="newPassword">The new password to hash and store.</param>
        /// <param name="user">The user record to update (but not save).</param>
        /// <param name="executionContext">
        /// The context of the command being executed. Used to capture the date, not to
        /// check permissions.
        /// </param>
        void UpdatePassword(string newPassword, User user, IExecutionContext executionContext);
    }
}
