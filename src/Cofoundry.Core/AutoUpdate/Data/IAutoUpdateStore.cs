using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate.Internal
{
    /// <summary>
    /// Database access for the auto update process, which may
    /// run before any tables are created.
    /// </summary>
    public interface IAutoUpdateStore
    {
        /// <summary>
        /// Gets a collections of module updates that have already been applied
        /// to the system. If the module update table does not exists then it
        /// is assumed that it's a fresh database and no updates have been made.
        /// </summary>
        Task<ICollection<ModuleVersion>> GetVersionHistoryAsync();

        /// <summary>
        /// Works out whether the database has been configured to be locked for schema 
        /// updates. This is different to distributed locking which is intended to prevent 
        /// multiple update instances running.
        /// </summary>
        Task<bool> IsDatabaseLockedAsync();

        /// <summary>
        /// Sets a flag in the database to enable/disable database updates. If
        /// schema updates are disabled then schema changes are expected to be applied 
        /// manually and the application will throw an exception if it discovers schema
        /// updates that it isn't permitted to apply.
        /// </summary>
        /// <param name="isLocked">
        /// <see langword="true"/> to lock the database and prevent schema updates.
        /// </param>
        Task SetDatabaseLockedAsync(bool isLocked);

        /// <summary>
        /// Logs that a module version update was applied successfully.
        /// </summary>
        /// <param name="module">The module that was updated.</param>
        /// <param name="version">The version of the module being applied.</param>
        /// <param name="description">The description of the update from the command that was executed.</param>
        Task LogSuccessAsync(string module, int version, string description);

        /// <summary>
        /// Logs that a module version update was unseccessful due to an error.
        /// </summary>
        /// <param name="module">The module that was attempting to update.</param>
        /// <param name="version">The version of the module being applied.</param>
        /// <param name="description">The description of the update from the command that was executed.</param>
        /// <param name="ex">The exception that was thrown when executing the update command.</param>
        Task LogErrorAsync(string module, int version, string description, Exception ex);
    }
}