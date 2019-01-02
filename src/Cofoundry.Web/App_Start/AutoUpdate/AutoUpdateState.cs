using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Singleton state object (managed through DI) that can be used
    /// to query the state of the auto updater.
    /// </summary>
    public class AutoUpdateState
    {
        /// <summary>
        /// The current state of the auto-update task.
        /// </summary>
        public AutoUpdateStatus Status { get; private set; }

        /// <summary>
        /// The exception that caused the stats to be put into the 
        /// AutoUpdateStatus.Error state.
        /// </summary>
        internal Exception Exception { get; private set; }

        /// <summary>
        /// Updated internally from the AutoUpdateHostedService to 
        /// update the status.
        /// </summary>
        /// <param name="newStatus">The new state.</param>
        /// <param name="ex">An exception can optionally be provded if the status is AutoUpdateStatus.Error</param>
        internal void Update(AutoUpdateStatus newStatus, Exception ex = null)
        {
            if (ex != null && newStatus != AutoUpdateStatus.Error)
            {
                throw new ArgumentException("An exception parameter can only be supplied if the status is AutoUpdateStatus.Error", nameof(ex));
            }

            Status = newStatus;
            Exception = ex;
        }
    }
}
