using Cofoundry.Domain;
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
    public class AddEndpointRoutesConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;
        private readonly PagesSettings _pagesSettings;

        public AddEndpointRoutesConfigurationTask(
            IRouteInitializer routeInitializer,
            PagesSettings pagesSettings
            )
        {
            _routeInitializer = routeInitializer;
            _pagesSettings = pagesSettings;
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
            app.UseEndpoints(GetRoutes);
        }

        #region helpers

        private void GetRoutes(IEndpointRouteBuilder routes)
        {
            RegisterInjectedRoutes(routes);

            if (!_pagesSettings.Disabled)
            {
                routes.MapControllerRoute(
                    "Cofoundry_Page",
                    "{**path}",
                    new { controller = "CofoundryPages", action = "Page" });
            }
        }

        private void RegisterInjectedRoutes(IEndpointRouteBuilder routes)
        {
            _routeInitializer.Initialize(routes);
        }

        #endregion
    }
}