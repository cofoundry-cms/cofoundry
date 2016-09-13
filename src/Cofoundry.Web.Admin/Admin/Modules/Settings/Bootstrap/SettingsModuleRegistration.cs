using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class SettingsModuleRegistration: IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFSET",
                Title = "Site Settings",
                Description = "Manage site settings.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Primary,
                Url = SettingsRouteLibrary.Urls.Details(),
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