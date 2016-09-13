using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class RolesModuleRegistrationduleRegistration : IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFROL",
                Title = "Roles & Permissions",
                Description = "Manage user roles and permissions.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = RolesRouteLibrary.Urls.List(),
                RestrictedToPermission = new RoleAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return RolesRouteLibrary.RoutePrefix; }
        }
    }
}