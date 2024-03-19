﻿namespace Cofoundry.Web.Admin;

public class AccountModuleRegistration : IInternalAngularModuleRegistration
{
    private readonly IAdminRouteLibrary _adminRouteLibrary;

    public AccountModuleRegistration(
        IAdminRouteLibrary adminRouteLibrary
        )
    {
        _adminRouteLibrary = adminRouteLibrary;
    }

    public AdminModule GetModule()
    {
        var module = new AdminModule()
        {
            AdminModuleCode = "COFACC",
            Title = "My Account",
            Description = "Manage your user account.",
            MenuCategory = AdminModuleMenuCategory.Settings,
            PrimaryOrdering = AdminModuleMenuPrimaryOrdering.Last,
            Url = _adminRouteLibrary.Account.Details()
        };

        return module;
    }

    public string RoutePrefix => AccountRouteLibrary.RoutePrefix;
}
