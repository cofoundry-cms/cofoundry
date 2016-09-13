using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// A handler for routing static resources in assemblies. Used so we
    /// can specify a route to a website content folder that should just serve 
    /// static files like images
    /// </summary>
    /// <remarks>
    /// adapted from http://www.codeproject.com/Articles/736830/Embedded-Resources-in-an-External-Lib-with-ASP-NET
    /// </remarks>
    public class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private RouteData _RouteData;
        public EmbeddedResourceHttpHandler(RouteData routeData)
        {
            _RouteData = routeData;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var path = context.Request.Url.AbsolutePath;

            if (HostingEnvironment.VirtualPathProvider.FileExists(path))
            {
                var file = HostingEnvironment.VirtualPathProvider.GetFile(path);

                if (file != null)
                {
                    using (var stream = file.Open())
                    {
                        context.Response.Clear();
                        context.Response.ContentType = MimeMapping.GetMimeMapping(path);
                        stream.CopyTo(context.Response.OutputStream);
                    }
                }
            }
        }
    }
}
