using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web.Admin;

/// <summary>
/// Used to write out script references to the angular admin UI for a standard implementation.
/// </summary>
public interface IAngularBootstrapper
{
    /// <summary>
    /// Adds scripts/templates for the core angular framework and the
    /// specified module and then bootstraps it.
    /// </summary>
    /// <param name="routeLibrary">Js routing library for the module to bootstrap.</param>
    /// <param name="options">
    /// Additional options object to render as js and inject as a constant in the angular module.
    /// </param>
    Task<IHtmlContent> BootstrapAsync(AngularModuleRouteLibrary routeLibrary, object? options = null);
}
