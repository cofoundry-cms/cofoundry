using Cofoundry.Core.Validation;
using Cofoundry.Domain.BackgroundTasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marks a user as deleted in the database (soft delete), removing personal 
    /// data fields and any optional relations from the UnstructuredDataDependency
    /// table. The remaining user record and relations are left in place for auditing.
    /// Log tables that contain IP references are not deleted, but should be
    /// cleared out periodically by the <see cref="UserCleanupBackgroundTask"/>.
    /// </summary>
    public class DeleteUserCommand : ICommand, ILoggableCommand
    {
        public DeleteUserCommand() { }

        /// <param name="userId">Id of the user to delete.</param>
        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Id of the user to delete.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }
    }
}