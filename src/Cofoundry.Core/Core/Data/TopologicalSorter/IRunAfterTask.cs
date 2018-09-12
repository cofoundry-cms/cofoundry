using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// This interface is used by the OrderableTaskSorter to order tasks
    /// based on other dependent tasks. Examples of this in use further up
    /// the stack are IRunAfterRouteRegistration and IRunAfterStartupTask.
    /// </summary>
    public interface IRunAfterTask
    {
        /// <summary>
        /// Indicates the types that this task should run 
        /// after when ordered.
        /// </summary>
        ICollection<Type> RunAfter { get; }
    }
}
