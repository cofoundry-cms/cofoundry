using System;
using System.Collections.Generic;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configuration class which can be used to customize the Cofoundry startup process. Most
    /// settings should be configured via the standard config files (which enabled transformations).
    /// </summary>
    public class UseCofoundryStartupConfiguration
    {
        /// <summary>
        /// A function that can be used to alter the startup task pipeline.
        /// </summary>
        public Func<IEnumerable<IStartupConfigurationTask>, IEnumerable<IStartupConfigurationTask>> StartupTaskFilter { get; set; }
    }
}