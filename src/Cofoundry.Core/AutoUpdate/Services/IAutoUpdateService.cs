using System;
using System.Threading;
using System.Threading.Tasks;
namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Service to update applications and modules.
    /// </summary>
    public interface IAutoUpdateService
    {
        /// <summary>
        /// Updates an application and referenced modules by scanning for implementations
        /// of IUpdatePackageFactory and executing any packages found.
        /// </summary>
        /// <param name="cancellationToken">
        /// Optional cancellation token that can be used to try and stop the update early, although
        /// a unit of updates will attempt to be completed before stopping.
        /// </param>
        Task UpdateAsync(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Runs a query to work out whether the database is locked for 
        /// schema updates.
        /// </summary>
        Task<bool> IsLockedAsync();

        /// <summary>
        /// Sets a flag in the database to enable/disable database updates.
        /// </summary>
        /// <param name="isLocked">True to lock the database and prevent schema updates</param>
        Task SetLockedAsync(bool isLocked);
    }
}
