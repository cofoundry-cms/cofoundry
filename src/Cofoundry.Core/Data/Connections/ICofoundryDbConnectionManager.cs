using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Cofoundry.Core.Data
{
    /// <summary>
    /// <para>
    /// Creates and manages connections to the Cofoundry database, with an option
    /// to use a shared connection that is persisted for the lifetime of the manager
    /// instance. This is useful for relying on a shared connection that can be enlisted
    /// in transactions without knowledge of any ambent transactions. 
    /// </para>
    /// <para>
    /// By default this is registered as scoped lifetime.
    /// </para>
    /// </summary>
    public interface ICofoundryDbConnectionManager
    {
        /// <summary>
        /// Creates a new connection to the Cofoundry database using the 
        /// connection string defined in config. You are responsible for
        /// managing and disposing of the connection.
        /// </summary>
        DbConnection Create();

        /// <summary>
        /// Returns a connection to the Cofoundry database that is shared
        /// across the liftime of the manager instance. The manager is responsible 
        /// for disposing of this instance; you should open and close the connection
        /// as needed but you should not dispose of it.
        /// </summary>
        DbConnection GetShared();
    }
}
