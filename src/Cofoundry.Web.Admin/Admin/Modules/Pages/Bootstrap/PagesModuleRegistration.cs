using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly PagesSettings _pagesSettings;

        public PagesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary,
            PagesSettings pagesSettings
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
            _pagesSettings = pagesSettings;
        }

        public AdminModule GetModule()
        {
            if (_pagesSettings.Disabled) return null;

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