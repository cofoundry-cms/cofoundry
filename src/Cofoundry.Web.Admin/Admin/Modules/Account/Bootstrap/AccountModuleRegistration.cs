using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class AccountModuleRegistration : IInternalAngularModuleRegistration
    {
        public AdminModule GetModule()
        {
            var module = new AdminModule()
            {
                AdminModuleCode = "COFACC",
                Title = "My Account",
                Description = "Manage your user account.",
                MenuCategory = AdminModuleMenuCategory.Settings,
                PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Last,
                Url = AccountRouteLibrary.Urls.Details()
            };

            return module;
        }

        public string RoutePrefix
        {
            get { return AccountRouteLibrary.RoutePrefix; }
        }
    }
}