using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
    public static class AdminRouteBuilderExtensions
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
        /// <param name="routeBuilder">The route builder to add routes to.</param>
        /// <param name="basePath">
        /// The base path of all routes on the controller, without the admin directory
        /// or any leading or trailing slashes e.g. "auth", "images", "users/roles"
        /// </param>
        /// <returns>Builder context that can be used to add routes.</returns>
        public static AdminRouteBuilderContext<TController> ForAdminController<TController>(this IEndpointRouteBuilder routeBuilder, string basePath)
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
        /// <param name="routeBuilder">The route builder to add routes to.</param>
        /// <param name="basePath">
        /// The base path of all routes on the controller, without the admin api directory
        /// or any leading or trailing slashes e.g. "auth", "images", "users/roles"
        /// </param>
        /// <returns>Api builder context that can be used to add routes.</returns>
        public static AdminApiRouteBuilderContext<TController> ForAdminApiController<TController>(this IEndpointRouteBuilder routeBuilder, string basePath)
            where TController : ControllerBase
        {
            var adminSettings = routeBuilder.ServiceProvider.GetRequiredService<AdminSettings>();
            return new AdminApiRouteBuilderContext<TController>(adminSettings, routeBuilder, basePath);
        }
    }
}
