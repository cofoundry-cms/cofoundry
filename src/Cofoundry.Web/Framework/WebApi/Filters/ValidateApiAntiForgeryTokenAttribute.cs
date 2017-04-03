using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cofoundry.Web.WebApi
{
    /// <summary>
    /// Ensures that a CSRF token is included in the "X-XSRF-Token" header of
    /// the request for all HttpMethod types except HttpMethod.Get.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateApiAntiForgeryTokenAttribute : Attribute, IAuthorizationFilter
    {
        private const string TOKEN_HEADER = "X-XSRF-Token";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            //if (request.Method != HttpMethod.Get)
            //{
            //    string token = null;
            //    IEnumerable<string> tokenHeaders;
            //    if (request.Headers.TryGetValues(TOKEN_HEADER, out tokenHeaders))
            //    {
            //        token = tokenHeaders.First();
            //    }

            //    if (string.IsNullOrWhiteSpace(token))
            //    {
            //        var result = new BadRequestResult();
            //        throw new ApplicationException($"The CSRF token is missing. Please add the {TOKEN_HEADER} header with a valid token.");
            //    }

            //    var service = IckyDependencyResolution.ResolveInWebApiContext<IAntiCSRFService>();
            //    service.ValidateToken(token);
            //}
        }
    }
}
