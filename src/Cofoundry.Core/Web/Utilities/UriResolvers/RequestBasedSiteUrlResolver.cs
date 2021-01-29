using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// A SiteUriResolver that uses the HttpContext.Current.Request object to work
    /// out the root path. This will fail if not used during an asp.net request so use
    /// an alternative resolver if this is a requirement.
    /// </summary>
    public class RequestBasedSiteUrlResolver : SiteUrlResolverBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestBasedSiteUrlResolver(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override string GetSiteRoot()
        {
            if (!CanResolve())
            {
                throw new InvalidOperationException($"HttpContext is not available, if you are trying to resolve a Uri outside of an request please use { typeof(ConfigBasedSiteUrlResolver).FullName } instead");
            }

            var request = _httpContextAccessor.HttpContext.Request;

            var siteRoot = string.Format("{0}://{1}{2}{3}",
                request.Scheme,
                request.Host.Host,
                GetPortUrlPart(request),
                request.PathBase
                );

            return siteRoot;
        }

        /// <summary>
        /// Indicates whether we arte in a web request and that
        /// we are able to resolve a url from HttpContext.Current.Request
        /// </summary>
        public bool CanResolve()
        {
            return _httpContextAccessor?.HttpContext?.Request != null;
        }

        private string GetPortUrlPart(HttpRequest request)
        {
            
            if (!request.Host.Port.HasValue 
                || request.Host.Port == 80 
                || (request.Host.Port == 443 && request.IsHttps))
            {
                return string.Empty;
            }

            return ":" + request.Host.Port;
        }
    }
}
