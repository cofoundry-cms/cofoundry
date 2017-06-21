using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DashboardModuleRegistration : IInternalAngularModuleRegistration
    {
        public const string ModuleCode = "COFDSH";
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public DashboardModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

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
                Url = _adminRouteLibrary.Dashboard.Dashboard(),
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