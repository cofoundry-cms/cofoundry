using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marks the user as deleted in the database (soft delete) and signs them out. Fields
    /// containing personal data are cleared and any optional relations from the UnstructuredDataDependency
    /// table are deleted. The remaining user record and relations are left in place for auditing.
    /// Log tables that contain IP references are not deleted, but should be
    /// cleared out periodically by the <see cref="BackgroundTasks.UserCleanupBackgroundTask"/>.
    /// </summary>
    public class DeleteCurrentUserCommand : ICommand, ILoggableCommand
    {
    }
}
