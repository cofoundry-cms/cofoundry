using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web.Admin
{
    public class AngularHelper
    {
        private readonly IAntiCSRFService _antiCSRFService;
        private readonly IStaticFilePathFormatter _staticFilePathFormatter;
        private readonly ICurrentUserViewHelper _currentUserHelper;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AngularHelper(
            IAntiCSRFService antiCSRFService,
            IStaticFilePathFormatter staticFilePathFormatter,
            ICurrentUserViewHelper currentUserHelper,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _antiCSRFService = antiCSRFService;
            _staticFilePathFormatter = staticFilePathFormatter;
            _currentUserHelper = currentUserHelper;
            _adminRouteLibrary = adminRouteLibrary;
        }

        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then bootstraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to bootstrap,</param>
        public IHtmlContent Bootstrap(ModuleRouteLibrary routeLibrary, object options = null)
        {
            var script = string.Concat(
                RenderScript(SharedRouteLibrary.Js.Main),
                RenderScript(SharedRouteLibrary.Js.Templates),
                RenderScript(routeLibrary.Main),
                RenderScript(routeLibrary.Templates),
                RenderBootstrapper(routeLibrary, options)
                );

            return new HtmlString(script);
        }

        #region private helpers

        private string RenderBootstrapper(ModuleRouteLibrary routeLibrary, object options)
        {
            var args = string.Empty;
            if (Debugger.IsAttached)
            {
                // use strict DI when in debug mode to throw up errors
                args = ", { strictDi: true }"; 
            }

            // Might need to add more info at some point, but right now we just need roles.
            var currentUserInfo = new {
                PermissionCodes = _currentUserHelper.Role.Permissions.Select(p => p.GetUniqueCode())
            };

            var csrfTopken = _antiCSRFService.GetToken();

            return @"<script>angular.element(document).ready(function() {
                        angular.module('" + _adminRouteLibrary.Shared.AngularModuleName + @"')
                               .constant('csrfToken', '" + csrfTopken + @"')"
                               + GetConstant(routeLibrary, "options", options) // not sure why the current module is loaded into the shared module - seems like a mistake?
                               + GetConstant(_adminRouteLibrary.Shared, "currentUser", currentUserInfo) + @";
                        angular.bootstrap(document, ['" + routeLibrary.AngularModuleName + "']" + args + @");
                    });</script>";
        }

        private static string GetOptions(ModuleRouteLibrary routeLibrary, object options)
        {
            return GetConstant(routeLibrary, "options", options);
        }

        private static string GetConstant<TValue>(ModuleRouteLibrary routeLibrary, string name, TValue value)
        {
            if (value != null)
            {
                var valueJson = JsonConvert.SerializeObject(value);
                return ".constant('" + routeLibrary.AngularModuleIdentifier + "." + name + "', " + valueJson + ")";
            }

            return string.Empty;
        }

        private string RenderScript(string path)
        {
            return $"<script src='{_staticFilePathFormatter.AppendVersion(path)}'></script>";
        }

        #endregion
    }
}