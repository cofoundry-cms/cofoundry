using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class UsersModuleRegistration : IAdminModuleRegistration
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public UsersModuleRegistration(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public IEnumerable<AdminModule> GetModules()
        {
            foreach (var userArea in _userAreaDefinitionRepository.GetAll())
            {
                var module = new AdminModule()
                {
                    AdminModuleCode = "COFUSR" + userArea.UserAreaCode,
                    Title = userArea.Name + " Users",
                    Description = "Manage users in the " + userArea.Name + " user area.",
                    MenuCategory = AdminModuleMenuCategory.Settings,
                    PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                    Url = _adminRouteLibrary.Users.List(userArea)
                };
                
                if (userArea is CofoundryAdminUserArea)
                {
                    module.RestrictedToPermission = new CofoundryUserAdminModulePermission();
                }
                else
                {
                    module.RestrictedToPermission = new NonCofoundryUserAdminModulePermission();
                }

                yield return module;
            }
        }

        public string RoutePrefix
        {
            get { return UsersRouteLibrary.RoutePrefix; }
        }
    }
}