using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Cofoundry.Web.ModularMvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web
{
    /// <summary>
    /// Misc setup configuration of the MVC framework.
    /// </summary>
    public class MvcConfigurationStartupTask : IStartupTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;
        private readonly IEnumerable<IEmbeddedResourceRouteRegistration> _routeRegistrations;

        public MvcConfigurationStartupTask(
            IRouteInitializer routeInitializer,
            IEnumerable<IEmbeddedResourceRouteRegistration> routeRegistrations
            )
        {
            _routeInitializer = routeInitializer;
            _routeRegistrations = routeRegistrations;
        }

        #endregion

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IApplicationBuilder app)
        {
            RegisterStaticFiles(app);
            app.UseMvc(GetRoutes);

            //ControllerBuilder.Current.DefaultNamespaces.Add(typeof(PagesController).Namespace);

            RegisterModelBinders();
        }

        #region helpers

        private void RegisterStaticFiles(IApplicationBuilder app)
        {
            // perhaps use a StaticFileOptions factory?
            app.UseStaticFiles(); // For the wwwroot folder

            foreach (var routeRegistration in _routeRegistrations)
            {
                var assembly = routeRegistration.GetType().Assembly;
                var fileProvider = new EmbeddedFileProvider(assembly);
                foreach (var route in routeRegistration.GetEmbeddedResourcePaths())
                {
                    app.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new FilteredEmbeddedFileProvider(fileProvider, route)
                    });
                }
            }
        }

        private static void RegisterModelBinders()
        {
            // TODO: Remove or relocate?
            //ModelBinders.Binders.Add(typeof(ImageAnchorLocation), new EnumBinder<ImageAnchorLocation>(null));
        }

        private void GetRoutes(IRouteBuilder routes)
        {
            RegisterRootFiles(routes);
            RegisterControllerRoutes(routes);
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