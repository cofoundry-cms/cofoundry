using Owin;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerRouteRegistrationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Late; }
        }

        public void Run(IAppBuilder app)
        {
            var routes = RouteTable.Routes;

            //Ensures this route executes first to inject siteviewer for all authenticated page requests (http://stackoverflow.com/a/6254124/486434)
            Route siteViewerRoute = routes.MapRoute(
                "Cofoundry_SiteViewer",
                "{*path}",
                new { controller = "Pages", action = "Page" },
                new { path = new SiteViewerRouteContraint() },
                new string[] { typeof(PagesController).Namespace });
            routes.Remove(siteViewerRoute);
            routes.Insert(0, siteViewerRoute);
        }
    }
}