using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// This interface is used by the OrderableTaskSorter to order tasks
    /// based on other dependent tasks. Examples of this in use further up
    /// the stack are IRunBeforeRouteRegistration and IRunBeforeStartupTask.
    /// </summary>
    public interface IRunBeforeTask
    {
        /// <summary>
        /// Indicates the types that this task should run 
        /// before when ordered.
        /// </summary>
        ICollection<Type> RunBefore { get; }
    }
}
