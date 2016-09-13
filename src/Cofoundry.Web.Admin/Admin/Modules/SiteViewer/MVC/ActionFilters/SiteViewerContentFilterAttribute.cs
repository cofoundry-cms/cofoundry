using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Adds a meta tag to a page that directs all links to the top frame
    /// </summary>
    public class SiteViewerContentFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var isInSiteViewerFrame = Convert.ToBoolean(filterContext.HttpContext.Request.QueryString["siteviewer"]);
                var response = filterContext.HttpContext.Response;

                if (isInSiteViewerFrame && response.ContentType == "text/html" && response.Filter != null && !filterContext.IsChildAction)
                {
                    response.Filter = new SiteViewerContentStream(response.Filter);
                }
            }
        }
    }

    
}