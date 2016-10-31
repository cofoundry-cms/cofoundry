using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Optimization;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public static class AngularHelper
    {
        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then boostraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to boostrap,</param>
        public static IHtmlString Bootstrap(ModuleJsRouteLibrary routeLibrary, ICurrentUserViewHelper currentUserHelper, object options = null)
        {
            var script = string.Concat(
                RenderScript(SharedRouteLibrary.Js.Main),
                RenderScript(SharedRouteLibrary.Js.Templates),
                RenderScript(routeLibrary.Main),
                RenderScript(routeLibrary.Templates),
                RenderBootstrapper(routeLibrary, currentUserHelper, options)
                );

            return new HtmlString(script);

        }

        #region private helpers

        private static string RenderBootstrapper(ModuleJsRouteLibrary routeLibrary, ICurrentUserViewHelper currentUserHelper, object options)
        {
            var args = string.Empty;
            if (Debugger.IsAttached)
            {
                // use strict DI when in debug mode to throw up errors
                args = ", { strictDi: true }"; 
            }

            // Might need to add more info at some point, but right now we just need roles.
            var currentUserInfo = new {
                PermissionCodes = currentUserHelper.Role.Permissions.Select(p => p.GetUniqueCode())
            };

            return @"<script>angular.element(document).ready(function() {
                        angular.module('" + SharedRouteLibrary.Js.AngularModuleName + @"')
                               .constant('csrfToken', '" + GetCsrfToken() + @"')"
                               + GetConstant(routeLibrary, "options", options) // not sure why the current module is loaded into the shared module - seems like a mistake?
                               + GetConstant(SharedRouteLibrary.Js, "currentUser", currentUserInfo) + @";
                        angular.bootstrap(document, ['" + routeLibrary.AngularModuleName + "']" + args + @");
                    });</script>";
        }

        private static string GetOptions(ModuleJsRouteLibrary routeLibrary, object options)
        {
            return GetConstant(routeLibrary, "options", options);
        }

        private static string GetConstant<TValue>(ModuleJsRouteLibrary routeLibrary, string name, TValue value)
        {
            if (value != null)
            {
                var valueJson = JsonConvert.SerializeObject(value);
                return ".constant('" + routeLibrary.AngularModuleIdentifier + "." + name + "', " + valueJson + ")";
            }

            return string.Empty;
        }

        private static string GetCsrfToken()
        {
            var service = new AntiCSRFService();
            return service.GetToken();
        }

        private static string RenderScript(string path)
        {
            return Scripts.Render(path).ToString();
        }

        #endregion
    }
}