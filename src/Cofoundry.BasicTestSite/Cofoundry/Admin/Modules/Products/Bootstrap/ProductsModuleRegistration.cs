using Cofoundry.Domain;
using Cofoundry.Web.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class ProductsModuleRegistration : IStandardAngularModuleRegistration
    {
        private readonly AdminSettings _adminSettings;

        public ProductsModuleRegistration(
            AdminSettings adminSettings
            )
        {
            _adminSettings = adminSettings;
        }

        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "BTPPRD",
                Title = "Products",
                Description = "Testing module.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Tertiary,
                Url = "/" + _adminSettings.DirectoryName + "/" + RoutePrefix
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return "products"; }
        }
    }
}
