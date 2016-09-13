using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Cofoundry.Web.ModularMvc;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesRouteRegistration : IRouteRegistration
    {
        private readonly ICustomEntityDefinition[] _customEntityModuleDefinition;

        public CustomEntitiesRouteRegistration(
            ICustomEntityDefinition[] customEntityModuleDefinition
            )
        {
            _customEntityModuleDefinition = customEntityModuleDefinition;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            var controllerNamespace = new string[] { typeof(CustomEntityModuleController).Namespace };

            foreach (var definition in _customEntityModuleDefinition)
            {
                var routePrefix = SlugFormatter.ToSlug(definition.NamePlural);
                var routeLibrary = new ModuleRouteLibrary(routePrefix);
                var jsRouteLibrary = new ModuleJsRouteLibrary(routeLibrary);

                routes.MapRoute(
                    "Custom Entity Admin Module - " + definition.NamePlural,
                    RouteConstants.AdminAreaPrefix + "/" + routePrefix,
                    new { controller = "CustomEntityModule", action = "Index", definition = definition, Area = RouteConstants.AdminAreaName },
                    controllerNamespace
                    );
            }
        }
    }
}
