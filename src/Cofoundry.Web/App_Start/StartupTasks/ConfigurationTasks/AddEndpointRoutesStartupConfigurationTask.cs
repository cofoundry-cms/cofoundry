using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web;

/// <summary>
/// Configures Cofoundry routing.
/// </summary>
public class AddEndpointRoutesStartupConfigurationTask : IStartupConfigurationTask
{
    private readonly IRouteInitializer _routeInitializer;
    private readonly PagesSettings _pagesSettings;

    public AddEndpointRoutesStartupConfigurationTask(
        IRouteInitializer routeInitializer,
        PagesSettings pagesSettings
        )
    {
        _routeInitializer = routeInitializer;
        _pagesSettings = pagesSettings;
    }

    public int Ordering
    {
        get
        {
            return (int)StartupTaskOrdering.Normal;
        }
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseEndpoints(GetRoutes);
    }

    private void GetRoutes(IEndpointRouteBuilder routes)
    {
        RegisterInjectedRoutes(routes);

        if (!_pagesSettings.Disabled)
        {
            routes.MapControllerRoute(
                "Cofoundry_Page",
                "{**path}",
                new { controller = "CofoundryPages", action = "Page" });
        }
    }

    private void RegisterInjectedRoutes(IEndpointRouteBuilder routes)
    {
        _routeInitializer.Initialize(routes);
    }
}
