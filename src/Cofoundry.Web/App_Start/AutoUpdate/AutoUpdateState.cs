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
        /// Updated internally from the AutoUpdateHostedService to 
        /// update the status.
        /// </summary>
        internal void Update(AutoUpdateStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
