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
    public class MvcStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IRouteInitializer _routeInitializer;
        private readonly PagesSettings _pagesSettings;

        public MvcStartupConfigurationTask(
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
            app.UseMvc(GetRoutes);
        }

        #region helpers

        private void GetRoutes(IRouteBuilder routes)
        {
            RegisterInjectedRoutes(routes);

            if (!_pagesSettings.Disabled)
            {
                routes.MapRoute(
                    "Cofoundry_Page",
                    "{**path}",
                    new { controller = "CofoundryPages", action = "Page" });
            }
        }

        private void RegisterInjectedRoutes(IRouteBuilder routes)
        {
            _routeInitializer.Initialize(routes);
        }

        #endregion
    }
}