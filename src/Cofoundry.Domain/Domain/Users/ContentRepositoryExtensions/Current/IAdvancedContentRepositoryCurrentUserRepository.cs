using System;
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
        /// Updates the user account of the currently logged in user.
        /// </summary>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with the current user data.
        /// </param>
        Task UpdateAsync(Action<UpdateCurrentUserCommand> commandPatcher);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// <see cref="UpdateCurrentUserPasswordCommand.OldPassword"/> field 
        /// to authenticate the request.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdatePasswordAsync(UpdateCurrentUserPasswordCommand command);

        /// <summary>
        /// Marks the user as deleted in the database (soft delete) and signs them out. Fields
        /// containing personal data are cleared and any optional relations from the UnstructuredDataDependency
        /// table are deleted. The remaining user record and relations are left in place for auditing.
        /// Log tables that contain IP references are not deleted, but should be
        /// cleared out periodically by the <see cref="BackgroundTasks.UserCleanupBackgroundTask"/>.
        /// </summary>
        Task DeleteAsync();
    }
}