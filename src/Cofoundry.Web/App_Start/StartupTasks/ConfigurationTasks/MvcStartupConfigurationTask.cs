using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds the ASP.Net MVC middleware to the pipeline and sets up Cofoundry routing.
    /// </summary>
    public class MvcStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;

        public MvcStartupConfigurationTask(
            IRouteInitializer routeInitializer
            )
        {
            _routeInitializer = routeInitializer;
        }

        #endregion

        public int Ordering
        {
            get
            {
                return (int)StartupTaskOrdering.Normal;
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(GetRoutes);
        }

        #region helpers

        private void GetRoutes(IRouteBuilder routes)
        {
            RegisterRootFiles(routes);
            RegisterControllerRoutes(routes);
        }

        private static void RegisterRootFiles(IRouteBuilder routes)
        {
            // General files
            routes.MapRoute("RobotsTxt", "robots.txt", new { controller = "CofoundryFiles", action = "RobotsTxt" });
            routes.MapRoute("HumansTxt", "humans.txt", new { controller = "CofoundryFiles", action = "HumansTxt" });
        }

        private void RegisterControllerRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "Cofoundry_ImageAsset",
                "assets/images/{assetId}_{fileName}.{extension}",
                new { controller = "CofoundryAssets", action = "Image" },
                new { assetId = @"\d+" });

            // File assets
            routes.MapRoute(
                "Cofoundry_DocumentAsset",
                "assets/files/{assetId}_{fileName}.{extension}",
                new { controller = "CofoundryAssets", action = "File" },
                new { assetId = @"\d+" });

            routes.MapRoute(
                "Cofoundry_ErrorCode",
                "cofoundryerror/errorcode/{statusCode}",
                new { controller = "CofoundryError", action = "ErrorCode" },
                new { statusCode = @"\d+" });

            routes.MapRoute(
                "Cofoundry_Exception",
                "cofoundryerror/exception/",
                new { controller = "CofoundryError", action = "Exception" });

            RegisterInjectedRoutes(routes);

            routes.MapRoute(
                "Cofoundry_Page",
                "{*path}",
                new { controller = "CofoundryPages", action = "Page" });
        }

        private void RegisterInjectedRoutes(IRouteBuilder routes)
        {
            _routeInitializer.Initialize(routes);
        }

        #endregion
    }
}