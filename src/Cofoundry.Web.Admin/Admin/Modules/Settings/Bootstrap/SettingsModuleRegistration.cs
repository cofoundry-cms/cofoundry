using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class SettingsModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SettingsModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFSET",
                Title = "Site Settings",
                Description = "Manage site settings.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Primary,
                Url = _adminRouteLibrary.Settings.Details(),
                RestrictedToPermission = new SettingsAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return SettingsRouteLibrary.RoutePrefix; }
        }
    }
}