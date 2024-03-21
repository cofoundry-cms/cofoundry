﻿using System.Text;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public class CofoundryFilesController : Controller
{
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

    public async Task<ActionResult> RobotsTxt()
    {
        string robotsTxt;

        if (_debugSettings.DisableRobotsTxt)
        {
            // Disallow when we're on the server but in debug mode
            robotsTxt = "User-agent: iisbot/1.0 (+http://www.iis.net/iisbot.html)\r\nAllow: /\r\nUser-agent: *\r\nDisallow: /";
        }
        else
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>());

            if (string.IsNullOrEmpty(settings.RobotsTxt))
            {
                Response.StatusCode = 404;
                return Content("Not found", "text/plain", Encoding.UTF8);
            }

            robotsTxt = settings.RobotsTxt;

            if (!robotsTxt.Contains("sitemap", StringComparison.OrdinalIgnoreCase))
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