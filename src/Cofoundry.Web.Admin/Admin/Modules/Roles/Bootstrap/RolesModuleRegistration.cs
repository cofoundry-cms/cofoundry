using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class RolesModuleRegistration : IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public RolesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFROL",
                Title = "Roles & Permissions",
                Description = "Manage user roles and permissions.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = _adminRouteLibrary.Roles.List(),
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