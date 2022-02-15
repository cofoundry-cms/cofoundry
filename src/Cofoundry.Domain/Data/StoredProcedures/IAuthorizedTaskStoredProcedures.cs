using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for authorized tasks.
    /// </summary>
    public interface IAuthorizedTaskStoredProcedures
    {
        /// <summary>
        /// Marks any non-complete authorized task records as invalid. Use this
        /// when a user completes an action that invalidates existing tasks, such as
        /// updating their password or contact addresses.
        /// </summary>
        /// <param name="userId">Id of the user to invalidate account recovery requests for.</param>
        /// <param name="dateNow">The current date and time to set the InvalidatedDate to.</param>
        Task InvalidateUserAccountRecoveryRequestsAsync(
            int userId,
            string[] AuthorizedTaskTypeCodes,
            DateTime dateNow
            );

        /// <summary>
        /// Removes completed, invalid or expired tasks from the database after a 
        /// period of time.
        /// </summary>
        /// <param name="completedItemRetentionTimeInSeconds">
        /// The amount of time in seconds to keep completed, invalid or expired tasks.
        /// </param>
        /// <param name="dateNow">The current date and time to set the InvalidatedDate to.</param>
        Task CleanupAsync(
            double retentionPeriodInSeconds,
            DateTime dateNow
            );
    }
}