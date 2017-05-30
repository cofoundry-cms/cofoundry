using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PageTemplatesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public PageTemplatesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public AdminModule GetModule()
        {
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