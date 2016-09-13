using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DirectoriesModuleRegistration: IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFDIR",
                Title = "Directories",
                Description = "Manage the directories in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = DirectoriesRouteLibrary.Urls.List(),
                RestrictedToPermission = new WebDirectoryAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return DirectoriesRouteLibrary.RoutePrefix; }
        }
    }
}