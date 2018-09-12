using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents a task that runs in the Cofoundry startup and 
    /// initialization pipeline. The task is dependent on another 
    /// task being executed first.
    /// </summary>
    public interface IRunAfterStartupConfigurationTask : IStartupConfigurationTask, IRunAfterTask
    {
    }
}