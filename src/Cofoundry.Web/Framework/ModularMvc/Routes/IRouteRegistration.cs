using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

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
        void RegisterRoutes(RouteCollection routes);
    }
}
