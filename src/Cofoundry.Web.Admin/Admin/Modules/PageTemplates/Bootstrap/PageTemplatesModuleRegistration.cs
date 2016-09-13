using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PageTemplatesModuleRegistration: IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFPGT",
                Title = "Page Templates",
                Description = "Manage templates for content pages.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = PageTemplatesRouteLibrary.Urls.List(),
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