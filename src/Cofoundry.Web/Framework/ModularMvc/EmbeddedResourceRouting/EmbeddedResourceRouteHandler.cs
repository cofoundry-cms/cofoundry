using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// A handler for routing resources embedded in assemblies. Used so we
    /// can specify a route to a website content folder that should just serve 
    /// static files like images
    /// </summary>
    /// <remarks>
    /// adapted from http://www.codeproject.com/Articles/736830/Embedded-Resources-in-an-External-Lib-with-ASP-NET
    /// </remarks>
    public class EmbeddedResourceRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(requestContext.RouteData);
        }
    }
}
