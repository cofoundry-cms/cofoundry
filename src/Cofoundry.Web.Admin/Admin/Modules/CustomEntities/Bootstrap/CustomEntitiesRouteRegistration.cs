using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Core;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesRouteRegistration : IRouteRegistration
    {
        private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinition;

        public CustomEntitiesRouteRegistration(
            IEnumerable<ICustomEntityDefinition> customEntityDefinition
            )
        {
            _customEntityDefinition = customEntityDefinition;
        }

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            foreach (var definition in _customEntityDefinition)
            {
                var routePrefix = SlugFormatter.ToSlug(definition.NamePlural);

                routeBuilder.MapRoute(
                    "Custom Entity Admin Module - " + definition.NamePlural,
                    RouteConstants.AdminAreaPrefix + "/" + routePrefix,
                    new { controller = "CustomEntityModule", action = "Index", Area = RouteConstants.AdminAreaName },
                    null,
                    new { Definition = definition }
                    );
            }
        }
    }
}
