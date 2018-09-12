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
    public class CustomEntitiesRouteRegistration : IOrderedRouteRegistration
    {
        private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinition;

        public CustomEntitiesRouteRegistration(
            IEnumerable<ICustomEntityDefinition> customEntityDefinition
            )
        {
            _customEntityDefinition = customEntityDefinition;
        }

        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            foreach (var definition in _customEntityDefinition)
            {
                var routePrefix = SlugFormatter.ToSlug(definition.NamePlural);

                routeBuilder.MapRoute(
                    "Cofoundry Admin Module - " + definition.NamePlural + " Custom Entity ",
                    RouteConstants.AdminAreaPrefix + "/" + routePrefix,
                    new { controller = "CustomEntityModule", action = "Index", Area = RouteConstants.AdminAreaName },
                    null,
                    new { Definition = definition }
                    );
            }
        }
    }
}
