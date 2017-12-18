using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Some sensible defaults for the numerical ordering value in a
    /// startup task.
    /// </summary>
    public enum StartupTaskOrdering
    {
        /// <summary>
        /// Mid-point ordering. This is when MvcStartupConfigurationTask runs.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Not used by the core framework, reserved for anything you need to run
        /// before the core framework starts.
        /// </summary>
        First = -400,

        /// <summary>
        /// Early startup tasks include registration of the AutoUpdateMiddleware,
        /// AuthStartupConfigurationTask and other key configuration.
        /// </summary>
        Early = -200,

        /// <summary>
        /// Runs after MVC and the core Cofoundry framework has been initialized.
        /// </summary>
        Late = 200,

        /// <summary>
        /// Not used by the core framework, reserved for anything that has to run 
        /// after everything else.
        /// </summary>
        Last = 400
    }
}