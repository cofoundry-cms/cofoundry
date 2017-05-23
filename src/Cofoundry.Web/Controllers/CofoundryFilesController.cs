using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Cofoundry.Web
{
    public class CofoundryFilesController : Controller
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ISiteUrlResolver _siteUriResolver;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        public CofoundryFilesController(
            IQueryExecutor queryExecutor,
            ISiteUrlResolver siteUriResolver,
            IHostingEnvironment hostingEnvironment
            )
        {
            _queryExecutor = queryExecutor;
            _siteUriResolver = siteUriResolver;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion

        public async Task<ActionResult> RobotsTxt()
        {
            string robotsTxt = "Sitemap: " + _siteUriResolver.MakeAbsolute("/sitemap.xml") + "\r\n";
            var robotsFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "robots.txt");

            if (_hostingEnvironment.IsDevelopment())
            {
                // Disallow when we're on the server but in debug mode
                robotsTxt += "User-agent: iisbot/1.0 (+http://www.iis.net/iisbot.html)\r\nAllow: /\r\nUser-agent: *\r\nDisallow: /";
            }
            else if (System.IO.File.Exists(robotsFilePath))
            {
                return File(robotsFilePath, "text/plain");
            }
            else
            {
                var settings = await _queryExecutor.GetAsync<SeoSettings>();
                robotsTxt += settings.RobotsTxt;

                if (string.IsNullOrEmpty(robotsTxt))
                {
                    Response.StatusCode = 404;
                    return Content("Not found", "text/plain", Encoding.UTF8);
                }
            }

            return Content(robotsTxt, "text/plain", Encoding.UTF8);
        }

        public async Task<ActionResult> HumansTxt()
        {
            var humansFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "humans.txt");

            if (System.IO.File.Exists(humansFilePath))
            {
                return File(humansFilePath, "text/plain");
            }

            var settings = await _queryExecutor.GetAsync<SeoSettings>();

            if (string.IsNullOrEmpty(settings.HumansTxt))
            {
                Response.StatusCode = 404;
                return Content("Not found", "text/plain", Encoding.UTF8);
            }

            return Content(settings.HumansTxt, "text/plain", Encoding.UTF8);
        }
    }
}
