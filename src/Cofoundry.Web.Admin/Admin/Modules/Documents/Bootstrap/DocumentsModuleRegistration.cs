using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DocumentsModuleRegistration : IInternalAngularModuleRegistration
    {
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly DocumentAssetsSettings _documentAssetsSettings;

        public DocumentsModuleRegistration(
            IAdminRouteLibrary adminRouteLibrary,
            DocumentAssetsSettings documentAssetsSettings
            )
        {
            _adminRouteLibrary = adminRouteLibrary;
            _documentAssetsSettings = documentAssetsSettings;
        }

        public AdminModule GetModule()
        {
            if (_documentAssetsSettings.Disabled) return null;

            var module = new AdminModule()
            {
                AdminModuleCode = "COFDOC",
                Title = "Documents",
                Description = "Manage the documents in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = _adminRouteLibrary.Documents.List(),
                RestrictedToPermission = new DocumentAssetAdminModulePermission()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return DocumentsModuleRouteLibrary.RoutePrefix; }
        }
    }
}