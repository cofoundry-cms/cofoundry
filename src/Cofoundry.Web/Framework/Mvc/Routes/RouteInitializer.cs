using Cofoundry.Core;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to inject all routes registered through the dependency
    /// container via IRouteRegistration. Override this if you want
    /// more control over the ordering of inject route registrations.
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

        private class RegistrationLookupItem
        {
            public RegistrationLookupItem(IRouteRegistration registrationLookup)
            {
                RouteRegistration = registrationLookup;
            }
            public IRouteRegistration RouteRegistration { get; set; }

            public List<IRouteRegistration> Dependencies { get; set; } = new List<IRouteRegistration>();
        }

        /// <summary>
        /// Orders and runs RegisterRoutes() on all instances of IRouteRegistration 
        /// registered in the dependency container.
        /// </summary>
        /// <param name="routeBuilder">
        /// The MVC routeBuilder to add routes to.
        /// </param>
        public void Initialize(IEndpointRouteBuilder routeBuilder)
        {
            ICollection<IRouteRegistration> sortedRoutes;

            try
            {
                // Then do a Topological Sort based on dependencies
                sortedRoutes = OrderableTaskSorter.Sort(_routeRegistrations);
            }
            catch (CyclicDependencyException ex)
            {
                throw new CyclicDependencyException($"A cyclic dependency has been detected between multiple {nameof(IRouteRegistration)} classes. Check your route registrations to ensure they do not depend on each other. For more details see the inner exception message.", ex);
            }

            foreach (var routeRegistration in sortedRoutes)
            {
                routeRegistration.RegisterRoutes(routeBuilder);
            }
        }
    }
}
