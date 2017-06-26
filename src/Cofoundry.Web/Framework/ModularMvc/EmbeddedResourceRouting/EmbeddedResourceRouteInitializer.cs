using Cofoundry.Core.EmbeddedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Adds mvc routes for content paths registered via
    /// classes that inherit from IEmbeddedContentRouteRegistration
    /// </summary>
    /// <remarks>
    /// This could be replaced with something that taps into Microsoft.Owin.FileSystems
    /// at some point
    /// </remarks>
    public class EmbeddedResourceRouteInitializer : IEmbeddedResourceRouteInitializer
    {
        private readonly IEmbeddedResourceRouteRegistration[] _routeRegistrations;

        public EmbeddedResourceRouteInitializer(
            IEmbeddedResourceRouteRegistration[] routeRegistrations
            )
        {
            _routeRegistrations = routeRegistrations;
        }

        /// <summary>
        /// Creates the embedded resource routes appending them to the
        /// MVC route table.
        /// </summary>
        public void Initialize()
        {
            var handler = new EmbeddedResourceRouteHandler();

            foreach (var routeRegistration in _routeRegistrations)
            foreach (var route in routeRegistration.GetEmbeddedResourcePaths())
            {
                var formattedRoute = route.Trim('/') + "/{*pathInfo}";
                
                // We need to supply a controller name to prevent it being picked up when asp.net generates routes for controllers
                var defaults = new RouteValueDictionary(new { controller = "DUMMMYVALUE" });
                RouteTable.Routes.Add(new Route(formattedRoute, defaults, handler));
            }
        }
    }
}
