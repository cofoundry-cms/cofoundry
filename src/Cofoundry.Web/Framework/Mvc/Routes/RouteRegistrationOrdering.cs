using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Some sensible defaults for the numerical ordering value to use
    /// in an IOrderableRouteRegistration.
    /// </summary>
    public enum RouteRegistrationOrdering
    {
        /// <summary>
        /// Mid-point ordering and the default ordering for registrations that do
        /// not have an ordering defined.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Not used by the core framework, reserved for any routes you need to run
        /// before the core framework is registered.
        /// </summary>
        First = -400,

        /// <summary>
        /// Early route registrations including the built-in Cofoundry routes
        /// for assets, admin panel and error pages.
        /// </summary>
        Early = -200,

        /// <summary>
        /// Runs after all the default registrations that have not specified an ordering.
        /// </summary>
        Late = 200,

        /// <summary>
        /// Not used by the core framework, reserved for anything that has to run 
        /// after everything else. The only exception to this is the Cofoundry page
        /// route which always runs after everything else because it is a catch all route
        /// that handles 404 pages.
        /// </summary>
        Last = 400
    }
}
