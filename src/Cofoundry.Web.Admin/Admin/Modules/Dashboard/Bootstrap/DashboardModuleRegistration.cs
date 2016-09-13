using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DashboardModuleRegistration : IInternalAngularModuleRegistration
    {
        public const string ModuleCode = "COFDSH";

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = ModuleCode,
                Title = "Dashboard",
                Description = "An overview of your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Primary,
                SecondaryOrdering = Int32.MaxValue,
                Url = DashboardRouteLibrary.Urls.Dashboard(),
                RestrictedToPermission = new DashboardAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return DashboardRouteLibrary.RoutePrefix; }
        }
    }
}