using Cofoundry.Domain;
using Cofoundry.Web.ModularMvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Misc setup configuration of the MVC framework.
    /// </summary>
    public class MvcConfigurationStartupTask : IStartupTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;
        private readonly IEmbeddedResourceRouteInitializer _embeddedResourceRouteInitializer;

        public MvcConfigurationStartupTask(
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

        public void Run(IApplicationBuilder app)
        {
            app.UseMvc(GetRoutes);

            //ControllerBuilder.Current.DefaultNamespaces.Add(typeof(PagesController).Namespace);

            RegisterModelBinders();
        }

        #region helpers

        private static void RegisterModelBinders()
        {
            // TODO: Remove or relocate?
            //ModelBinders.Binders.Add(typeof(ImageAnchorLocation), new EnumBinder<ImageAnchorLocation>(null));
        }

        private void GetRoutes(IRouteBuilder routes)
        {
            SetupStaticFileRouting(routes);
            RegisterRootFiles(routes);
            RegisterControllerRoutes(routes);
        }

        private void SetupStaticFileRouting(IRouteBuilder routes)
        {
            _embeddedResourceRouteInitializer.Initialize();
        }

        private static void RegisterRootFiles(IRouteBuilder routes)
        {
            // General files
            routes.MapRoute("RobotsTxt", "robots.txt", new { controller = "Files", action = "RobotsTxt" });
            routes.MapRoute("HumansTxt", "humans.txt", new { controller = "Files", action = "HumansTxt" });
        }

        private void RegisterControllerRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "Cofoundry_ImageAsset",
                "assets/images/{assetId}_{fileName}.{extension}",
                new { controller = "Assets", action = "Image" },
                new { assetId = @"\d+" });

            // File assets
            routes.MapRoute(
                "Cofoundry_DocumentAsset",
                "assets/files/{assetId}_{fileName}.{extension}",
                new { controller = "Assets", action = "File" },
                new { assetId = @"\d+" });

            RegisterInjectedRoutes(routes);

            routes.MapRoute(
                "Cofoundry_Page",
                "{*path}",
                new { controller = "Pages", action = "Page" });
        }

        private void RegisterInjectedRoutes(IRouteBuilder routes)
        {
            _routeInitializer.Initialize(routes);
        }

        #endregion
    }
}