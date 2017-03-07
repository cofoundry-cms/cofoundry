using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Cofoundry.Web.WebApi
{
    /// <summary>
    /// Ensures that a CSRF token is included in the "X-XSRF-Token" header of
    /// the request for all HttpMethod types except HttpMethod.Get.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateApiAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        private const string TOKEN_HEADER = "X-XSRF-Token";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method != HttpMethod.Get)
            {
                string token = null;
                IEnumerable<string> tokenHeaders;
                if (actionContext.Request.Headers.TryGetValues(TOKEN_HEADER, out tokenHeaders))
                {
                    token = tokenHeaders.First();
                }

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new ApplicationException($"The CSRF token is missing. Please add the {TOKEN_HEADER} header with a valid token.");
                }
                
                var service = IckyDependencyResolution.ResolveInWebApiContext<IAntiCSRFService>();
                service.ValidateToken(token);
            }
        }
    }
}
