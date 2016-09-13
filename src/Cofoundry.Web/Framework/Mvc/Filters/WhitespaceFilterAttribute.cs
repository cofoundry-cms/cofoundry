using Cofoundry.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Action filter attribute for removing white space in html response. Can be turned off in the web.config.
    /// </summary>
    public class WhitespaceFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // No constructor injection available on filters
            var optimizationSettings = IckyDependencyResolution.ResolveFromMvcContext<OptimizationSettings>();

            if (optimizationSettings.RemoveWhitespaceFromHtml)
            {
                // Only remove white space for HTML documents
                var response = filterContext.HttpContext.Response;
                if (response.ContentType == "text/html" && response.Filter != null && !filterContext.IsChildAction)
                {
                    response.Filter = new WhitespaceStream(response.Filter);
                }
            }
        }
    }
}