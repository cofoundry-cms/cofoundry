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
        private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinitions;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public CustomEntitiesAdminModuleRegistration(
            IEnumerable<ICustomEntityDefinition> customEntityDefinitions,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _customEntityDefinitions = customEntityDefinitions;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<AdminModule> GetModules()
        {
            foreach (var definition in _customEntityDefinitions)
            {
                var module = new AdminModule()
                {
                    AdminModuleCode = definition.CustomEntityDefinitionCode,
                    Description = definition.Description,
                    MenuCategory = AdminModuleMenuCategory.ManageSite,
                    PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Tertiary,
                    Title = definition.NamePlural,
                    Url = _adminRouteLibrary.CustomEntities.List(definition),
                    RestrictedToPermission = new CustomEntityAdminModulePermission(definition)
                };

                yield return module;
            }
        }
    }
}
