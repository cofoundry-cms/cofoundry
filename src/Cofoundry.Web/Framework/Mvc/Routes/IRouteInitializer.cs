using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web;

/// <summary>
/// Used to inject all routes registered through the dependency
/// container via IRouteRegistration. Override this if you want
/// more control over the ordering of inject route registrations.
/// </summary>
public interface IRouteInitializer
{
    /// <summary>
    /// Orders and runs RegisterRoutes() on all instances of IRouteRegistration 
    /// registered in the dependency container.
    /// </summary>
    /// <param name="routes">
    /// The MVC IEndpointRouteBuilder to add routes to.
    /// </param>
    void Initialize(IEndpointRouteBuilder routes);
}
