using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class AdminMenuViewComponent : ViewComponent
{
    private readonly IQueryExecutor _queryExecutor;

    public AdminMenuViewComponent(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menuItems = await _queryExecutor.ExecuteAsync(new GetPermittedAdminModulesQuery());
        var vm = new AdminMenuViewModel();

        vm.ManageSiteModules = menuItems
            .Where(m => m.MenuCategory == AdminModuleMenuCategory.ManageSite)
            .SetStandardOrdering()
            .ToList();

        vm.SettingsModules = menuItems
            .Where(m => m.MenuCategory == AdminModuleMenuCategory.Settings)
            .SetStandardOrdering()
            .ToList();

        var selectedItem = menuItems
            .Select(m => new
            {
                Module = m,
                Link = m.GetMenuLinkByPath(Request.Path)
            })
            .Where(m => m.Link != null)
            .FirstOrDefault();

        if (selectedItem != null)
        {
            vm.SelectedCategory = selectedItem.Module.MenuCategory;
            vm.SelectedModule = selectedItem.Module;
        }

        var viewPath = ViewPathFormatter.View("Shared", "Components/AdminMenu");

        return View(viewPath, vm);
    }
}
