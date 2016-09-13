using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class UsersModuleRegistration : IAdminModuleRegistration
    {
        private readonly IUserAreaRepository _userAreaRepository;

        public UsersModuleRegistration(
            IUserAreaRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public IEnumerable<AdminModule> GetModules()
        {
            foreach (var userArea in _userAreaRepository.GetAll())
            {
                var routeLibrary = new ModuleRouteLibrary(SlugFormatter.ToSlug(userArea.Name) + "-users");

                var module = new AdminModule()
                {
                    AdminModuleCode = "COFUSR" + userArea.UserAreaCode,
                    Title = userArea.Name + " Users",
                    Description = "Manage users in the " + userArea.Name + " user area.",
                    MenuCategory = AdminModuleMenuCategory.Settings,
                    PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                    Url = routeLibrary.CreateAngularRoute()
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