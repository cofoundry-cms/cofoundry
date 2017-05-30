using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public PagesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFPAG",
                Title = "Pages",
                Description = "Manage the pages in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Primary,
                Url = _adminRouteLibrary.Pages.List(),
                RestrictedToPermission = new PageAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return PagesModuleRouteLibrary.RoutePrefix; }
        }
    }
}