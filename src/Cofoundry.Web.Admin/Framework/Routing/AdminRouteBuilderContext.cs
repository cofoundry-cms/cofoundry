using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A chainable-context used to add a batch of admin panel MVC routes to a 
    /// single controller. Routes specified through this builder will be 
    /// automatically registered with the correct admin panel path, which
    /// can be altered via the Cofoundry:Admin:DirectoryName setting.
    /// </summary>
    public class AdminRouteBuilderContext<TController>
        where TController : Controller
    {
        private readonly AdminSettings _adminSettings;
        private readonly IEndpointRouteBuilder _routeBuilder;
        private readonly string _basePath;

        public AdminRouteBuilderContext(
            AdminSettings adminSettings,
            IEndpointRouteBuilder routeBuilder,
            string basePath
            )
        {
            _adminSettings = adminSettings;
            _routeBuilder = routeBuilder;
            _basePath = basePath;
        }

        /// <summary>
        /// Maps a request to the base path and the "Index" action.
        /// </summary>
        /// <param name="dataTokens">
        /// An object that contains data tokens for the route. The object's 
        /// properties represent the names and values of the data tokens.
        /// </param>
        public AdminRouteBuilderContext<TController> MapIndexRoute(object dataTokens = null)
        {
            if (_adminSettings.Disabled) return this;

            string controllerName = GetControllerName();

            _routeBuilder.MapControllerRoute(
                "Cofoundry Admin - " + _basePath,
                _adminSettings.DirectoryName + "/" + _basePath,
                new { controller = controllerName, action = "Index", Area = RouteConstants.AdminAreaName },
                null,
                dataTokens
                );

            return this;
        }

        /// <summary>
        /// Maps a request to a specific action, where the path value
        /// is the same as the "slug" version of the action name e.g. 
        /// action "Edit" has a path value of "edit" and action "ResetPassword" 
        /// has a path value of "reset-password".
        /// </summary>
        /// <param name="action">
        /// The action name to bind to, which will also be lowercased and 
        /// used as the path value e.g. action "Edit" has a path value of 
        /// "edit" and action "ResetPassword" has a path value of "reset-password".
        /// </param>
        /// <param name="dataTokens">
        /// An object that contains data tokens for the route. The object's 
        /// properties represent the names and values of the data tokens.
        /// </param>
        public AdminRouteBuilderContext<TController> MapRoute(
            string action,
            object dataTokens = null
            )
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentEmptyException();
            return MapRoute(action, SlugFormatter.CamelCaseToSlug(action), dataTokens);
        }

        /// <summary>
        /// Maps a request to a specific path and action
        /// </summary>
        /// <param name="action">The action name to bind to e.g. "Edit" or "ResetPassword".</param>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "edit" or "reset-password".
        /// </param>
        /// <param name="dataTokens">
        /// An object that contains data tokens for the route. The object's 
        /// properties represent the names and values of the data tokens.
        /// </param>
        public AdminRouteBuilderContext<TController> MapRoute(
            string action,
            string path,
            object dataTokens = null
            )
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentEmptyException();

            if (_adminSettings.Disabled) return this;

            var controllerName = GetControllerName();
            var fullPath = _adminSettings.DirectoryName + "/" + _basePath;

            if (!string.IsNullOrWhiteSpace(path))
            {
                fullPath += "/" + path;
            }

            _routeBuilder.MapControllerRoute(
                "Cofoundry Admin - " + fullPath,
                fullPath,
                new { controller = controllerName, action = action, Area = RouteConstants.AdminAreaName },
                null,
                dataTokens
                );

            return this;
        }

        private static string GetControllerName()
        {
            const string CONTROLLER_SUFFIX = "Controller";
            var controllerName = typeof(TController).Name;

            if (controllerName.EndsWith(CONTROLLER_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                controllerName = controllerName.Remove(controllerName.Length - CONTROLLER_SUFFIX.Length);
            }

            return controllerName;
        }
    }
}
