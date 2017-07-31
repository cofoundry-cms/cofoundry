using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DirectoriesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public DirectoriesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
        }

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFDIR",
                Title = "Directories",
                Description = "Manage the directories in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = _adminRouteLibrary.Directories.List(),
                RestrictedToPermission = new PageDirectoryAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return DirectoriesRouteLibrary.RoutePrefix; }
        }
    }
}