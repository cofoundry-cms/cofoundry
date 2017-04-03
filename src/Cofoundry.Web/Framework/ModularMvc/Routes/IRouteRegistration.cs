using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Use this to inject route registrations into the 
    /// Route Collection. The ordering that instances of IRouteRegistration
    /// get run cannot be guaranteed.
    /// </summary>
    public interface IRouteRegistration
    {
        /// <summary>
        /// Register routes with the mvc RouteCollection. 
        /// </summary>
        void RegisterRoutes(IRouteBuilder routes);
    }
}
