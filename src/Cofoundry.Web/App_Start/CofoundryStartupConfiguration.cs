using System;
using System.Collections.Generic;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configuration class which can be used to customize the Cofoundry startup process. Most
    /// settings should be configured via the standard config files (which enabled transformations).
    /// </summary>
    public class CofoundryStartupConfiguration
    {
        /// <summary>
        /// A function that can be used to alter the startup task pipeline.
        /// </summary>
        public Func<IEnumerable<IStartupTask>, IEnumerable<IStartupTask>> StartupTaskFilter { get; private set; }

        /// <summary>
        /// Use this to remove/append/wrap or re-order the startup task collection as
        /// you please.
        /// </summary>
        /// <param name="startupTaskFilter">A function that returns a collection of startup tasks to run.</param>
        /// <returns></returns>
        public CofoundryStartupConfiguration FilterStartupTasks(Func<IEnumerable<IStartupTask>, IEnumerable<IStartupTask>> startupTaskFilter)
        {
            StartupTaskFilter = startupTaskFilter;

            return this;
        }
    }
}