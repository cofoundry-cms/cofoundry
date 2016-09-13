using System;
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
        /// <remarks>
        /// Async because sometimes UpdateCommands need to be async to save having to create
        /// sync versions of methods that would not normally require them. E.g. when calling into
        /// shared command handlers. I don't really think there's much benefit in making any other
        /// part asyn because nothing else useful should be happening while the db update is going on anyway.
        /// </remarks>
        Task UpdateAsync();

        /// <summary>
        /// Runs a query to work out whether the database is locked for 
        /// schema updates.
        /// </summary>
        bool IsLocked();

        /// <summary>
        /// Sets a flag in the database to enable/disable database updates.
        /// </summary>
        /// <param name="isLocked">True to lock the database and prevent schema updates</param>
        void SetLocked(bool isLocked);
    }
}
