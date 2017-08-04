using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initializer for registering routes
    /// </summary>
    public interface IRouteInitializer
    {
        /// <summary>
        /// Runs RegisterRoutes() on all instances of IRouteRegistration 
        /// registered in the dependency container.
        /// </summary>
        void Initialize(IRouteBuilder routes);
    }
}
