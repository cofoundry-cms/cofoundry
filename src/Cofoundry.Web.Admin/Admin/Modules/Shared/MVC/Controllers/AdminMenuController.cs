using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [RoutePrefix("AdminMenu")]
    [Route("{action=MainMenu}")]
    public class AdminMenuController : BaseAdminMvcController
    {
        private readonly IQueryExecutor _queryExecutor;

        public AdminMenuController(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public PartialViewResult MainMenu()
        {
            var menuItems = _queryExecutor.Execute(new GetPermittedAdminModulesQuery());
            var vm = new AdminMenuViewModel();

            vm.ManageSiteModules = menuItems
                .Where(m => m.MenuCategory == AdminModuleMenuCategory.ManageSite)
                .SetStandardOrdering();

            vm.SettingsModules = menuItems
                .Where(m => m.MenuCategory == AdminModuleMenuCategory.Settings)
                .SetStandardOrdering();

            var selectedItem = menuItems
                .Select(m => new {
                    Module = m,
                    Link = m.GetMenuLinkByPath(Request.Url.LocalPath)
                })
                .Where(m => m.Link != null)
                .FirstOrDefault();

            if (selectedItem != null)
            {
                vm.SelectedCategory = selectedItem.Module.MenuCategory;
                vm.SelectedModule = selectedItem.Module;
            }

            return PartialView(vm);
        }
    }
}