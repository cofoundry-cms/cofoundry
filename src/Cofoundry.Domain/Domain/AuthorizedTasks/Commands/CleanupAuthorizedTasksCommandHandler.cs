using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data.Internal;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Removes completed, invalid or expired tasks from the database after a 
    /// period of time.
    /// </summary>
    public class CleanupAuthorizedTasksCommandHandler
        : ICommandHandler<CleanupAuthorizedTasksCommand>
        , ICofoundryUserPermissionCheckHandler
    {
        private readonly IAuthorizedTaskStoredProcedures _authorizedTaskStoredProcedures;

        public CleanupAuthorizedTasksCommandHandler(
            IAuthorizedTaskStoredProcedures authorizedTaskStoredProcedures
            )
        {
            _authorizedTaskStoredProcedures = authorizedTaskStoredProcedures;
        }

        public Task ExecuteAsync(CleanupAuthorizedTasksCommand command, IExecutionContext executionContext)
        {
            return _authorizedTaskStoredProcedures.CleanupAsync(command.RetentionPeriod.TotalSeconds);
        }
    }
}