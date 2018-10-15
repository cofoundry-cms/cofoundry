using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PageTemplatesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly PagesSettings _pagesSettings;

        public PageTemplatesModuleRegistration(
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
                AdminModuleCode = "COFPGT",
                Title = "Page Templates",
                Description = "Manage templates for content pages.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = _adminRouteLibrary.PageTemplates.List(),
                RestrictedToPermission = new PageTemplateAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return PageTemplatesRouteLibrary.RoutePrefix; }
        }
    }
}