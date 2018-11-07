using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Indicates the state of the auto-update task that runs at startup.
    /// </summary>
    public enum AutoUpdateStatus
    {
        /// <summary>
        /// The auto-updater has not yet started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The auto-updater has started but not finished.
        /// </summary>
        InProgress,

        /// <summary>
        /// The auto-updater is already being run by another process (e.g.
        /// another server instance) and cannot be started yet. The updater
        /// will be retried soon.
        /// </summary>
        LockedByAnotherProcess,

        /// <summary>
        /// The auto-updater has completed successfully.
        /// </summary>
        Complete,

        /// <summary>
        /// An error has occured in the auto-updater task and the will be attempted again soon.
        /// </summary>
        Error
    }
}
