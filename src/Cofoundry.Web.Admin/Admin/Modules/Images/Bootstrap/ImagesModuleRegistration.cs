using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class ImagesModuleRegistration: IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFIMG",
                Title = "Images",
                Description = "Manage the images in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = ImagesRouteLibrary.Urls.List(),
                RestrictedToPermission = new ImageAssetAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return ImagesRouteLibrary.RoutePrefix; }
        }
    }
}