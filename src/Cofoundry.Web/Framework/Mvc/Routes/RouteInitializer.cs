using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initializer for registering routes
    /// </summary>
    public class RouteInitializer : IRouteInitializer
    {
        private readonly IEnumerable<IRouteRegistration> _routeRegistrations;

        public RouteInitializer(
            IEnumerable<IRouteRegistration> routeRegistrations
            )
        {
            _routeRegistrations = routeRegistrations;
        }

        /// <summary>
        /// Runs RegisterRoutes() on all instances of IRouteRegistration 
        /// registered in the dependency container.
        /// </summary>
        public void Initialize(IRouteBuilder routes)
        {
            foreach (var routeRegistration in _routeRegistrations)
            {
                routeRegistration.RegisterRoutes(routes);
            }
        }
    }
}
