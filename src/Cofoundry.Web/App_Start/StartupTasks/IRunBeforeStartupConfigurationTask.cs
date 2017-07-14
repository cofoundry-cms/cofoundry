using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents a task that runs in the Cofoundry startup and 
    /// initialization pipeline. This interface indicates that
    /// the task should run before other tasks specified in the RunBefore
    /// property.
    /// </summary>
    public interface IRunBeforeStartupConfigurationTask : IStartupConfigurationTask
    {
        /// <summary>
        /// Indicates the type of task that this task should run before.
        /// </summary>
        Type[] RunBefore { get; }
    }
}