using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    public class FilesController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ISiteUrlResolver _siteUriResolver;

        #region Constructors

        public FilesController(
            IQueryExecutor queryExecutor,
            ISiteUrlResolver siteUriResolver
            )
        {
            _queryExecutor = queryExecutor;
            _siteUriResolver = siteUriResolver;
        }

        #endregion

        public ActionResult RobotsTxt()
        {
            string robotsTxt = "Sitemap: " + _siteUriResolver.MakeAbsolute("/sitemap.xml") + "\r\n";

            if (HttpContext.IsDebuggingEnabled && Request.Url.Host.ToLower() != "localhost")
            {
                // Disallow when we're on the server but in debug mode
                robotsTxt += "User-agent: iisbot/1.0 (+http://www.iis.net/iisbot.html)\r\nAllow: /\r\nUser-agent: *\r\nDisallow: /";
            }
            else if (System.IO.File.Exists(Server.MapPath("~/robots.txt")))
            {
                return File(Server.MapPath("~/robots.txt"), "text/plain");
            }
            else
            {
                var settings = _queryExecutor.Get<SeoSettings>();
                robotsTxt += settings.RobotsTxt;

                if (string.IsNullOrEmpty(robotsTxt))
                {
                    Response.StatusCode = 404;
                    return Content("Not found", "text/plain", Encoding.UTF8);
                }
            }

            return Content(robotsTxt, "text/plain", Encoding.UTF8);
        }

        public ActionResult HumansTxt()
        {
            if (System.IO.File.Exists(Server.MapPath("~/humans.txt")))
            {
                return File(Server.MapPath("~/humans.txt"), "text/plain");
            }

            var settings = _queryExecutor.Get<SeoSettings>();

            if (string.IsNullOrEmpty(settings.HumansTxt))
            {
                Response.StatusCode = 404;
                return Content("Not found", "text/plain", Encoding.UTF8);
            }

            return Content(settings.HumansTxt, "text/plain", Encoding.UTF8);
        }
    }
}
