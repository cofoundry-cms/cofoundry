using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class DocumentsModuleRegistration : IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFDOC",
                Title = "Documents",
                Description = "Manage the documents in your site.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Secondry,
                Url = DocumentsModuleRouteLibrary.Urls.List(),
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