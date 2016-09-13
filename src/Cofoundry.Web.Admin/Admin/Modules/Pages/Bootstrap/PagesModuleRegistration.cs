using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRegistration: IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFPAG",
                Title = "Pages",
                Description = "Manage the pages in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Primary,
                Url = PagesRouteLibrary.Urls.List(),
                RestrictedToPermission = new PageAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return PagesRouteLibrary.RoutePrefix; }
        }
    }
}