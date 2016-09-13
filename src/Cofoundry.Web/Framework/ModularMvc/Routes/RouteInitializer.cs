using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Initializer for registering routes
    /// </summary>
    public class RouteInitializer : IRouteInitializer
    {
        private readonly IRouteRegistration[] _routeRegistrations;

        public RouteInitializer(
            IRouteRegistration[] routeRegistrations
            )
        {
            _routeRegistrations = routeRegistrations;
        }

        /// <summary>
        /// Runs RegisterRoutes() on all instances of IRouteRegistration 
        /// registered in the dependency container.
        /// </summary>
        public void Initialize(RouteCollection routes)
        {
            foreach (var routeRegistration in _routeRegistrations)
            {
                routeRegistration.RegisterRoutes(routes);
            }
        }
    }
}
