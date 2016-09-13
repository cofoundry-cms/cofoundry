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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateApiAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method != HttpMethod.Get)
            {
                string token = null;
                IEnumerable<string> tokenHeaders;
                if (actionContext.Request.Headers.TryGetValues("X-XSRF-Token", out tokenHeaders))
                {
                    token = tokenHeaders.First();
                }

                var service = new AntiCSRFService();
                service.ValidateToken(token);
            }
        }
    }
}
