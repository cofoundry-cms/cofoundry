using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Implement this interface to define routes that should be
    /// injected into the MVC route collection during the startup 
    /// process. IRunAfterRouteRegistration or IRunBeforeRouteRegistration
    /// can be used to affect the ordering of registrations, but otherwise
    /// the ordering that instances of IRouteRegistration get run cannot 
    /// is not guaranteed.
    /// </summary>
    public interface IRouteRegistration
    {
        /// <summary>
        /// Register routes with the mvc RouteCollection. 
        /// </summary>
        void RegisterRoutes(IRouteBuilder routeBuilder);
    }
}
