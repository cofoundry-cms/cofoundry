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
using System.IO;
using Cofoundry.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Used to write out script references to the angular admin UI for a standard implementation
    /// </summary>
    public class AngularBootstrapper : IAngularBootstrapper
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly IUserContextService _userContextService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DebugSettings _debugSettings;
        private readonly AdminSettings _adminSettings;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public AngularBootstrapper(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            IUserContextService userContextService,
            IAdminRouteLibrary adminRouteLibrary,
            IQueryExecutor queryExecutor,
            IWebHostEnvironment webHostEnvironment,
            DebugSettings debugSettings,
            AdminSettings adminSettings,
            JsonSerializerSettings jsonSerializerSettings
            )
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _userContextService = userContextService;
            _adminRouteLibrary = adminRouteLibrary;
            _queryExecutor = queryExecutor;
            _webHostEnvironment = webHostEnvironment;
            _debugSettings = debugSettings;
            _adminSettings = adminSettings;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then bootstraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to bootstrap.</param>
        /// <param name="options">
        /// Additional options object to render as js and inject as a constant in the angular module.
        /// </param>
        public async Task<IHtmlContent> BootstrapAsync(AngularModuleRouteLibrary routeLibrary, object options = null)
        {
            var bootstrapScript = await RenderBootstrapperAsync(routeLibrary, options);

            var formattedPluginScripts = GetPluginScripts();

            var scripts = new List<string>();
            AddScript(scripts, _adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.MainScriptName);
            AddScript(scripts, _adminRouteLibrary.Shared, _adminRouteLibrary.Shared.Angular.TemplateScriptName);
            scripts.AddRange(formattedPluginScripts);
            AddScript(scripts, _adminRouteLibrary.SharedAlternate, _adminRouteLibrary.SharedAlternate.Angular.MainScriptName, true);
            AddScript(scripts, _adminRouteLibrary.SharedAlternate, _adminRouteLibrary.SharedAlternate.Angular.TemplateScriptName, true);
            AddScript(scripts, routeLibrary, routeLibrary.Angular.MainScriptName);
            AddScript(scripts, routeLibrary, routeLibrary.Angular.TemplateScriptName);
            scripts.Add(bootstrapScript);

            var script = string.Concat(scripts);

            return new HtmlString(script);
        }

        #region private helpers

        private void AddScript(
            List<string> scriptToAddTo, 
            ModuleRouteLibrary moduleRouteLibrary, 
            string fileName, 
            bool checkIfResourceExists = false
            )
        {
            HtmlString script;

            if (checkIfResourceExists)
            {
                script = _staticResourceReferenceRenderer.ScriptTagIfExists(moduleRouteLibrary, fileName);
            }
            else
            {
                script = _staticResourceReferenceRenderer.ScriptTag(moduleRouteLibrary, fileName);
            }

            if (script != null)
            {
                scriptToAddTo.Add(script.ToString());
            }
        }

        /// <summary>
        /// To support multiple plugins, we have to scan for multiple files
        /// in the plugin shared script directory.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetPluginScripts()
        {
            var formattedPluginScripts = _staticResourceReferenceRenderer
                .ScriptTagsForDirectory(_adminRouteLibrary.SharedPlugin)
                .Select(s => s.ToString())
                ;

            return formattedPluginScripts;
        }

        private async Task<string> RenderBootstrapperAsync(AngularModuleRouteLibrary routeLibrary, object options)
        {
            var args = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
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
            var canShowDeveloperException = _debugSettings.CanShowDeveloperExceptionPage(_webHostEnvironment);

            return @"<script>angular.element(document).ready(function() {
                        angular.module('" + _adminRouteLibrary.Shared.Angular.AngularModuleName + @"')
                               .constant('csrfToken', '" + tokens.RequestToken + @"')
                               .constant('csrfHeaderName', '" + tokens.HeaderName + @"')"
                               + GetConstant(_adminRouteLibrary.Shared, "showDevException", canShowDeveloperException)
                               + GetConstant(_adminRouteLibrary.Shared, "serviceBase", "/" + _adminSettings.DirectoryName + "/api/") 
                               + GetConstant(_adminRouteLibrary.Shared, "pluginServiceBase", "/" + _adminSettings.DirectoryName + "/api/plugins/") 
                               + GetConstant(_adminRouteLibrary.Shared, "urlBaseBase", "/" + _adminSettings.DirectoryName + "/")
                               + GetConstant(_adminRouteLibrary.Shared, "internalContentPath", _adminRouteLibrary.Shared.GetStaticResourceUrlPath() + "/")
                               + GetConstant(_adminRouteLibrary.Shared, "pluginContentPath", _adminRouteLibrary.SharedPlugin.GetStaticResourceUrlPath() + "/")
                               + GetConstant(_adminRouteLibrary.Shared, "contentPath", _adminRouteLibrary.SharedAlternate.GetStaticResourceUrlPath() + "/")
                               + GetConstant(routeLibrary, "options", options) // not sure why the current module is loaded into the shared module - seems like a mistake?
                               + GetConstant(_adminRouteLibrary.Shared, "currentUser", currentUserInfo) + @";
                        angular.bootstrap(document, ['" + routeLibrary.Angular.AngularModuleName + "']" + args + @");
                    });</script>";
        }

        private string GetOptions(AngularModuleRouteLibrary routeLibrary, object options)
        {
            return GetConstant(routeLibrary, "options", options);
        }

        private string GetConstant<TValue>(AngularModuleRouteLibrary routeLibrary, string name, TValue value)
        {
            if (value != null)
            {
                var valueJson = JsonConvert.SerializeObject(value, _jsonSerializerSettings);
                return ".constant('" + routeLibrary.Angular.AngularModuleIdentifier + "." + name + "', " + valueJson + ")";
            }

            return string.Empty;
        }

        #endregion
    }
}