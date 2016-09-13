using Cofoundry.Web.ModularMvc;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configures MVC and Cofoundry content routes, and also adds any 
    /// routes added via the ModularMvc framework.
    /// </summary>
    public class MvcRouteConfigurationStartupTask : IStartupTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;
        private readonly IEmbeddedResourceRouteInitializer _embeddedResourceRouteInitializer;

        public MvcRouteConfigurationStartupTask(
            IRouteInitializer routeInitializer,
            IEmbeddedResourceRouteInitializer embeddedResourceRouteInitializer
            )
        {
            _routeInitializer = routeInitializer;
            _embeddedResourceRouteInitializer = embeddedResourceRouteInitializer;
        }

        #endregion

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IAppBuilder app)
        {
            var routes = RouteTable.Routes;

            SetupAttributeRouting(routes);
            SetupStaticFileRouting(routes);
            RegisterIgnores(routes);
            RegisterRootFiles(routes);
            RegisterControllerRoutes(routes);
        }

        #region helpers

        private static void SetupAttributeRouting(RouteCollection routes)
        {
            // Set up attribute routing - must be done before Areas
            routes.MapMvcAttributeRoutes();
        }

        private void SetupStaticFileRouting(RouteCollection routes)
        {
            // runs requests for static files through the routing system
            // (I'm not sure the specific significance of this yet)
            routes.RouteExistingFiles = true;

            _embeddedResourceRouteInitializer.Initialize();
        }

        private static void RegisterIgnores(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"^(.*/)?favicon\.([pP][nN][gG]|[iI][cC][oO])$" });
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("google{random}.html");
            routes.IgnoreRoute("BingSiteAuth.xml");
        }

        private static void RegisterRootFiles(RouteCollection routes)
        {
            // General files
            routes.MapRoute("RobotsTxt", "robots.txt", new { controller = "Files", action = "RobotsTxt" });
            routes.MapRoute("HumansTxt", "humans.txt", new { controller = "Files", action = "HumansTxt" });
        }

        private void RegisterControllerRoutes(RouteCollection routes)
        {
            var controllerNamespace = new string[] { typeof(PagesController).Namespace };

            routes.MapRoute(
                "ImageAsset",
                "assets/images/{assetId}_{fileName}.{extension}",
                new { controller = "Assets", action = "Image" },
                new { assetId = @"\d+" },
                controllerNamespace);

            // File assets
            routes.MapRoute(
                "DocumentAsset",
                "assets/files/{assetId}_{fileName}.{extension}",
                new { controller = "Assets", action = "File" },
                new { assetId = @"\d+" },
                controllerNamespace);

            RegisterInjectedRoutes(routes);

            routes.MapRoute(
                "Page",
                "{*path}",
                new { controller = "Pages", action = "Page" },
                controllerNamespace);
        }

        private void RegisterInjectedRoutes(RouteCollection routes)
        {
            _routeInitializer.Initialize(routes);
        }

        #endregion
    }
}