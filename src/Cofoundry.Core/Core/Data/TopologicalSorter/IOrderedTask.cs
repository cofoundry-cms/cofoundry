using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// This interface is used by the OrderableTaskSorter to order tasks
    /// based on an integer ordering value. Examples of this in use further up
    /// the stack are IStartupTask and IOrderedRouteRegistration.
    /// </summary>
    public interface IOrderedTask
    {
        /// <summary>
        /// An integer representing the ordering (lower values first). In typical
        /// implementations there is an enum that can be used to used to for 
        /// recommended values e.g. StartupTaskOrdering, RouteRegistrationTaskOrdering
        /// </summary>
        int Ordering { get; }
    }
}
