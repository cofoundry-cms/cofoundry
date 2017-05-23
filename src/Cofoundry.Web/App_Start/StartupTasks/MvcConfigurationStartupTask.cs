using Cofoundry.Core.ResourceFiles;
using Cofoundry.Web.ModularMvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core;
using Microsoft.Extensions.Options;
using Cofoundry.Domain;

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
        private readonly IUserAreaRepository _userAreaRepository;

        public MvcConfigurationStartupTask(
            IRouteInitializer routeInitializer,
            IEnumerable<IEmbeddedResourceRouteRegistration> routeRegistrations,
            IUserAreaRepository userAreaRepository
            )
        {
            _routeInitializer = routeInitializer;
            _routeRegistrations = routeRegistrations;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IApplicationBuilder app)
        {
            RegisterStaticFiles(app);

            foreach (var userAreaDefinition in _userAreaRepository.GetAll())
            {
                var cookieOptions = new CookieAuthenticationOptions();
                cookieOptions.AuthenticationScheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaDefinition.UserAreaCode);

                // Share the cookie name between user areas, because you should only be able to log into one at a time,
                cookieOptions.CookieName = "CF_AUTH";

                // NB: When adding multiple authentication middleware you should ensure that no middleware is configured to run automatically
                // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme
                //cookieOptions.AutomaticAuthenticate = false;

                if (!string.IsNullOrWhiteSpace(userAreaDefinition.LoginPath))
                {
                    cookieOptions.LoginPath = userAreaDefinition.LoginPath;
                }

                if (!string.IsNullOrWhiteSpace(userAreaDefinition.LogoutPath))
                {
                    cookieOptions.LogoutPath = userAreaDefinition.LogoutPath;
                }

                if (!string.IsNullOrWhiteSpace(userAreaDefinition.AccessDeniedPath))
                {
                    cookieOptions.AccessDeniedPath = userAreaDefinition.AccessDeniedPath;
                }

                app.UseCookieAuthentication(cookieOptions);
            }

            app.UseMvc(GetRoutes);

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