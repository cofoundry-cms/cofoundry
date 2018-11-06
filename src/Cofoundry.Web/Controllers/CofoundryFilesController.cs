using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    public class CofoundryFilesController : Controller
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ISiteUrlResolver _siteUriResolver;
        private readonly DebugSettings _debugSettings;

        public CofoundryFilesController(
            IQueryExecutor queryExecutor,
            ISiteUrlResolver siteUriResolver,
            DebugSettings debugSettings
            )
        {
            _queryExecutor = queryExecutor;
            _siteUriResolver = siteUriResolver;
            _debugSettings = debugSettings;
        }

        #endregion

        public async Task<ActionResult> RobotsTxt()
        {
            string robotsTxt = string.Empty;

            if (_debugSettings.DisableRobotsTxt)
            {
                // Disallow when we're on the server but in debug mode
                robotsTxt = "User-agent: iisbot/1.0 (+http://www.iis.net/iisbot.html)\r\nAllow: /\r\nUser-agent: *\r\nDisallow: /";
            }
            else
            {
                var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>());
                robotsTxt = settings.RobotsTxt;

                if (string.IsNullOrEmpty(robotsTxt))
                {
                    Response.StatusCode = 404;
                    return Content("Not found", "text/plain", Encoding.UTF8);
                }

                if (robotsTxt.IndexOf("sitemap", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    robotsTxt += "\r\nSitemap: " + _siteUriResolver.MakeAbsolute("/sitemap.xml");
                }
            }

            return Content(robotsTxt, "text/plain", Encoding.UTF8);
        }

        public async Task<ActionResult> HumansTxt()
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>());

            if (string.IsNullOrEmpty(settings.HumansTxt))
            {
                Response.StatusCode = 404;
                return Content("Not found", "text/plain", Encoding.UTF8);
            }

            return Content(settings.HumansTxt, "text/plain", Encoding.UTF8);
        }
    }
}
