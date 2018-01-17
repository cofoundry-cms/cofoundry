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
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "BTPPRD",
                Title = "Products",
                Description = "Testing module.",
                MenuCategory = AdminModuleMenuCategory.ManageSite,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Tertiary,
                Url = RouteConstants.AdminUrlRoot + "/" + RoutePrefix
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return "products"; }
        }
    }
}
