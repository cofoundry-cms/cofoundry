using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A chainable-context used to add a batch of admin panel api routes to a 
    /// single controller. Routes specified through this builder will be 
    /// automatically registered with the correct admin panel api path, which
    /// can be altered via the Cofoundry:Admin:DirectoryName setting.
    /// </summary>
    public class AdminApiRouteBuilderContext<TController>
        where TController : ControllerBase
    {
        private readonly AdminSettings _adminSettings;
        private readonly IEndpointRouteBuilder _routeBuilder;
        private readonly string _basePath;

        public AdminApiRouteBuilderContext(
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
        /// Maps a GET request to the base path, mapping to the "Get"
        /// action.
        /// </summary>
        public AdminApiRouteBuilderContext<TController> MapGet()
        {
            AddGetRoute(_basePath);

            return this;
        }

        /// <summary>
        /// Maps a GET request to the specified path, mapping to the "GetById"
        /// action.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "{myId:int}".
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapGetById(string path)
        {
            MapGet(path, "GetById");

            return this;
        }

        /// <summary>
        /// Maps a GET request to the specified path and optionally an action 
        /// name. If the action name is not specified then the default "Get" is
        /// used.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "products/{myId:int}".
        /// </param>
        /// <param name="actionName">
        /// Optional name of the action on the controller to bind to. If the action
        /// name is not specified then the default "Get" is used.
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapGet(string path, string actionName = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

            AddGetRoute(_basePath + "/" + path, actionName);

            return this;
        }

        /// <summary>
        /// Maps a POST request to the base path, mapping to the "Post"
        /// action.
        /// </summary>
        public AdminApiRouteBuilderContext<TController> MapPost()
        {
            AddPostRoute(_basePath);

            return this;
        }

        /// <summary>
        /// Maps a POST request to the specified path and optionally an action 
        /// name. If the action name is not specified then the default "Post" is
        /// used.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "products/{myId:int}".
        /// </param>
        /// <param name="actionName">
        /// Optional name of the action on the controller to bind to. If the action
        /// name is not specified then the default "Post" is used.
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapPost(string path, string actionName = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

            AddPostRoute(_basePath + "/" + path, actionName);

            return this;
        }

        /// <summary>
        /// Maps a PUT request to the base path, mapping to the "Put"
        /// action.
        /// </summary>
        public AdminApiRouteBuilderContext<TController> MapPut()
        {
            AddPutRoute(_basePath);

            return this;
        }

        /// <summary>
        /// Maps a PUT request to the specified path and optionally an action 
        /// name. If the action name is not specified then the default "Put" is
        /// used.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "products/{myId:int}".
        /// </param>
        /// <param name="actionName">
        /// Optional name of the action on the controller to bind to. If the action
        /// name is not specified then the default "Put" is used.
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapPut(string path, string actionName = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

            AddPutRoute(_basePath + "/" + path, actionName);

            return this;
        }

        /// <summary>
        /// Maps a PATCH request to the base path, mapping to the "Patch"
        /// action.
        /// </summary>
        public AdminApiRouteBuilderContext<TController> MapPatch()
        {
            AddPatchRoute(_basePath);

            return this;
        }

        /// <summary>
        /// Maps a PATCH request to the specified path and optionally an action 
        /// name. If the action name is not specified then the default "Patch" is
        /// used.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "products/{myId:int}".
        /// </param>
        /// <param name="actionName">
        /// Optional name of the action on the controller to bind to. If the action
        /// name is not specified then the default "Patch" is used.
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapPatch(string path, string actionName = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

            AddPatchRoute(_basePath + "/" + path, actionName);

            return this;
        }

        /// <summary>
        /// Maps a DELETE request to the base path, mapping to the "Delete"
        /// action.
        /// </summary>
        public AdminApiRouteBuilderContext<TController> MapDelete()
        {
            AddDeleteRoute(_basePath);

            return this;
        }

        /// <summary>
        /// Maps a DELETE request to the specified path and optionally an action 
        /// name. If the action name is not specified then the default "Delete" is
        /// used.
        /// </summary>
        /// <param name="path">
        /// The path segment to add to the base url and bind the request to
        /// e.g. "products/{myId:int}".
        /// </param>
        /// <param name="actionName">
        /// Optional name of the action on the controller to bind to. If the action
        /// name is not specified then the default "Delete" is used.
        /// </param>
        public AdminApiRouteBuilderContext<TController> MapDelete(string path, string actionName = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

            AddDeleteRoute(_basePath + "/" + path, actionName);

            return this;
        }

        #region private helpers

        private void AddGetRoute(string path, string actionName = null)
        {
            AddVerbRoute("Get", path, actionName);
        }

        private void AddPostRoute(string path, string actionName = null)
        {
            AddVerbRoute("Post", path, actionName);
        }

        private void AddPutRoute(string path, string actionName = null)
        {
            AddVerbRoute("Put", path, actionName);
        }

        private void AddPatchRoute(string path, string actionName = null)
        {
            AddVerbRoute("Patch", path, actionName);
        }

        private void AddDeleteRoute(string path, string actionName = null)
        {
            AddVerbRoute("Delete", path, actionName);
        }

        private void AddVerbRoute(string verbActionName, string path, string actionName)
        {
            if (_adminSettings.Disabled) return;
            var verb = verbActionName.ToUpper();

            if (string.IsNullOrEmpty(actionName))
            {
                actionName = verbActionName;
            }

            string controllerName = GetControllerName();

            _routeBuilder.MapControllerRoute(
                $"Cofoundry Admin Module - {verb}: {path}",
                _adminSettings.DirectoryName + "/api/" + path,
                new { controller = controllerName, action = actionName, Area = RouteConstants.AdminAreaName },
                constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(verb) })
                );
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

        #endregion
    }
}
