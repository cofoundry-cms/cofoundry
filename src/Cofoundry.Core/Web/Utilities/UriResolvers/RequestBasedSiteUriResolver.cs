using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// A SiteUriResolver that uses the HttpContext.Current.Request object to work
    /// out the root path. This will fail if not used during an asp.net request so use
    /// an alternative resolver if this is a requirement.
    /// </summary>
    public class RequestBasedSiteUriResolver : SiteUriResolverBase
    {
        protected override string GetSiteRoot()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
            {
                throw new InvalidOperationException("HttpContext.Current.Request is null, if you are trying to resolve a Uri outside of an request please use SiteUriResolver instead");
            }

            var request = HttpContext.Current.Request;
            
            var siteRoot = string.Format("{0}://{1}{2}{3}",
                request.Url.Scheme,
                request.Url.Host,
                GetPortUrlPart(request),
                request.ApplicationPath);

            return siteRoot;
        }

        private string GetPortUrlPart(HttpRequest request)
        {
            if (request.Url.Port == 80 || (request.Url.Port == 443 && request.Url.Scheme == "https"))
            {
                return string.Empty;
            }

            return ":" + request.Url.Port;
        }
    }
}
