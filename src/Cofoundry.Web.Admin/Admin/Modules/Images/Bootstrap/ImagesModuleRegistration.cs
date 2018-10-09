using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class ImagesModuleRegistration: IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly ImageAssetsSettings _imageAssetsSettings;

        public ImagesModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary,
            ImageAssetsSettings imageAssetsSettings
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
            _imageAssetsSettings = imageAssetsSettings;
        }

        public AdminModule GetModule()
        {
            if (_imageAssetsSettings.Disabled) return null;

            var module = new AdminModule()
            {
                AdminModuleCode = "COFIMG",
                Title = "Images",
                Description = "Manage the images in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = _adminRouteLibrary.Images.List(),
                RestrictedToPermission = new ImageAssetAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return ImagesModuleRouteLibrary.RoutePrefix; }
        }
    }
}