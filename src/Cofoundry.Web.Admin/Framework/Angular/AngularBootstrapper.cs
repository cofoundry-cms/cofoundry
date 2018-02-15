using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class AngularBootstrapper : IAngularBootstrapper
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly IUserContextService _userContextService;
        private readonly IQueryExecutor _queryExecutor;

        public AngularBootstrapper(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            IUserContextService userContextService,
            IAdminRouteLibrary adminRouteLibrary,
            IQueryExecutor queryExecutor
            )
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _userContextService = userContextService;
            _adminRouteLibrary = adminRouteLibrary;
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then bootstraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to bootstrap,</param>
        public async Task<IHtmlContent> BootstrapAsync(AngularModuleRouteLibrary routeLibrary, object options = null)
        {
            var bootstrapScript = await RenderBootstrapperAsync(routeLibrary, options);

            var script = string.Concat(
                _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.MainScriptName),
                _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.TemplateScriptName),
                _staticResourceReferenceRenderer.ScriptTagIfExists(_adminRouteLibrary.SharedAlternate, _adminRouteLibrary.SharedAlternate.Angular.MainScriptName),
                _staticResourceReferenceRenderer.ScriptTagIfExists(_adminRouteLibrary.SharedAlternate, _adminRouteLibrary.SharedAlternate.Angular.TemplateScriptName),
                _staticResourceReferenceRenderer.ScriptTag(routeLibrary, routeLibrary.Angular.MainScriptName),
                _staticResourceReferenceRenderer.ScriptTag(routeLibrary, routeLibrary.Angular.TemplateScriptName),
                bootstrapScript
                );

            return new HtmlString(script);
        }

        #region private helpers

        private async Task<string> RenderBootstrapperAsync(AngularModuleRouteLibrary routeLibrary, object options)
        {
            var args = string.Empty;
            if (Debugger.IsAttached)
            {
                // use strict DI when in debug mode to throw up errors
                args = ", { strictDi: true }"; 
            }

            // Might need to add more info at some point, but right now we just need roles.
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.AreaCode);
            var role = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(user.RoleId));
            var currentUserInfo = new {
                PermissionCodes = role
                    .Permissions
                    .Select(p => p.GetUniqueCode())
                    .ToList()
            };

            var tokens = _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext);

            return @"<script>angular.element(document).ready(function() {
                        angular.module('" + _adminRouteLibrary.Shared.Angular.AngularModuleName + @"')
                               .constant('csrfToken', '" + tokens.RequestToken + @"')
                               .constant('csrfHeaderName', '" + tokens.HeaderName + @"')"
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