using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesAdminModuleRegistration : IAdminModuleRegistration
    {
        private readonly ICustomEntityDefinition[] _customEntityModuleDefinition;

        public CustomEntitiesAdminModuleRegistration(
            ICustomEntityDefinition[] customEntityModuleDefinition
            )
        {
            _customEntityModuleDefinition = customEntityModuleDefinition;
        }

        public IEnumerable<AdminModule> GetModules()
        {
            foreach (var definition in _customEntityModuleDefinition)
            {
                var routeLibrary = new ModuleRouteLibrary(SlugFormatter.ToSlug(definition.NamePlural));

                var module = new AdminModule()
                {
                    AdminModuleCode = definition.CustomEntityDefinitionCode,
                    Description = definition.Description,
                    MenuCategory = AdminModuleMenuCategory.ManageSite,
                    PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Tertiary,
                    Title = definition.NamePlural,
                    Url = routeLibrary.CreateAngularRoute()
                };

                yield return module;
            }
        }
    }
}
