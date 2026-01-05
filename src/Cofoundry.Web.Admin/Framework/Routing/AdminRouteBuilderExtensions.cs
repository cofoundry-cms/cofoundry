using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web.Admin;

/// <summary>
/// <see cref="IEndpointRouteBuilder"/> extensions for adding admin panel routes.
/// </summary>
public static class AdminRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        /// <summary>
        /// Returns a builder context than can be used to add admin panel routes 
        /// to the specified MVC controller. Routes specified through this method
        /// will automatically be registered with the correct admin panel path, which
        /// can be altered via the Cofoundry:Admin:DirectoryName setting.
        /// </summary>
        /// <typeparam name="TController">
        /// The controller type to add routes for. This type is used to scope the
        /// routes to the controller.
        /// </typeparam>
        /// <param name="basePath">
        /// The base path of all routes on the controller, without the admin directory
        /// or any leading or trailing slashes e.g. "auth", "images", "users/roles"
        /// </param>
        /// <returns>Builder context that can be used to add routes.</returns>
        public AdminRouteBuilderContext<TController> ForAdminController<TController>(string basePath)
            where TController : Controller
        {
            var adminSettings = routeBuilder.ServiceProvider.GetRequiredService<AdminSettings>();
            return new AdminRouteBuilderContext<TController>(adminSettings, routeBuilder, basePath);
        }

        /// <summary>
        /// Returns a builder context than can be used to add admin panel api
        /// routes to the specified controller. Routes specified through this method
        /// will automatically be registered with the correct admin panel api path, which
        /// can be altered via the Cofoundry:Admin:DirectoryName setting.
        /// </summary>
        /// <typeparam name="TController">
        /// The controller type to add routes for. This type is used to scope the
        /// api routes to the controller.
        /// </typeparam>
        /// <param name="basePath">
        /// The base path of all routes on the controller, without the admin api directory
        /// or any leading or trailing slashes e.g. "auth", "images", "users/roles"
        /// </param>
        /// <returns>Api builder context that can be used to add routes.</returns>
        public AdminApiRouteBuilderContext<TController> ForAdminApiController<TController>(string basePath)
            where TController : ControllerBase
        {
            var adminSettings = routeBuilder.ServiceProvider.GetRequiredService<AdminSettings>();
            return new AdminApiRouteBuilderContext<TController>(adminSettings, routeBuilder, basePath);
        }
    }
}
