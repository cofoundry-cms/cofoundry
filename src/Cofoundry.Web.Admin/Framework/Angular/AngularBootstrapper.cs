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
    public class AngularBootstrapper : IAngularBootstrapper
    {
        private readonly IAntiCSRFService _antiCSRFService;
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly ICurrentUserViewHelper _currentUserHelper;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AngularBootstrapper(
            IAntiCSRFService antiCSRFService,
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            ICurrentUserViewHelper currentUserHelper,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _antiCSRFService = antiCSRFService;
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _currentUserHelper = currentUserHelper;
            _adminRouteLibrary = adminRouteLibrary;
        }

        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then bootstraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to bootstrap,</param>
        public IHtmlContent Bootstrap(AngularModuleRouteLibrary routeLibrary, object options = null)
        {
            var script = string.Concat(
                _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.MainScriptName),
                _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.TemplateScriptName),
                _staticResourceReferenceRenderer.ScriptTag(routeLibrary, routeLibrary.Angular.MainScriptName),
                _staticResourceReferenceRenderer.ScriptTag(routeLibrary, routeLibrary.Angular.TemplateScriptName),
                RenderBootstrapper(routeLibrary, options)
                );

            return new HtmlString(script);
        }

        #region private helpers

        private string RenderBootstrapper(AngularModuleRouteLibrary routeLibrary, object options)
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
                        angular.module('" + _adminRouteLibrary.Shared.Angular.AngularModuleName + @"')
                               .constant('csrfToken', '" + csrfTopken + @"')"
                               + GetConstant(routeLibrary, "options", options) // not sure why the current module is loaded into the shared module - seems like a mistake?
                               + GetConstant(_adminRouteLibrary.Shared, "currentUser", currentUserInfo) + @";
                        angular.bootstrap(document, ['" + routeLibrary.Angular.AngularModuleName + "']" + args + @");
                    });</script>";
        }

        private static string GetOptions(AngularModuleRouteLibrary routeLibrary, object options)
        {
            return GetConstant(routeLibrary, "options", options);
        }

        private static string GetConstant<TValue>(AngularModuleRouteLibrary routeLibrary, string name, TValue value)
        {
            if (value != null)
            {
                var valueJson = JsonConvert.SerializeObject(value);
                return ".constant('" + routeLibrary.Angular.AngularModuleIdentifier + "." + name + "', " + valueJson + ")";
            }

            return string.Empty;
        }

        #endregion
    }
}