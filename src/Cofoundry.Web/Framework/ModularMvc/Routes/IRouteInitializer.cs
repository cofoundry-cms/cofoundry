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
    public interface IRouteInitializer
    {
        /// <summary>
        /// Runs RegisterRoutes() on all instances of IRouteRegistration 
        /// registered in the dependency container.
        /// </summary>
        void Initialize(RouteCollection routes);
    }
}
