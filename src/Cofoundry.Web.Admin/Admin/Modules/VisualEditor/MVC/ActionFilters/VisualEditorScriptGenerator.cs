using Cofoundry.Core.Json;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// Used to generate the scripts and CSS links that are added
    /// to a page in order display the visual editor.
    /// </summary>
    public class VisualEditorScriptGenerator : IVisualEditorScriptGenerator
    {
        const char TAB = '\t';
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly IPageResponseDataCache _pageResponseDataCache;
        private readonly IRazorViewRenderer _razorViewRenderer;
        private readonly IResourceLocator _resourceLocator;
        private readonly IPermissionValidationService _permissionValidationService;

        public VisualEditorScriptGenerator(
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            IAdminRouteLibrary adminRouteLibrary,
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            IPageResponseDataCache pageResponseDataCache,
            IRazorViewRenderer razorViewRenderer,
            IResourceLocator resourceLocator,
            IPermissionValidationService permissionValidationService
            )
        {
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _adminRouteLibrary = adminRouteLibrary;
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _pageResponseDataCache = pageResponseDataCache;
            _razorViewRenderer = razorViewRenderer;
            _resourceLocator = resourceLocator;
            _permissionValidationService = permissionValidationService;
        }

        /// <summary>
        /// Generates any scripts or CSS links that need to be
        /// added to the document header.
        /// </summary>
        public string CreateHeadScript()
        {
            return TAB + _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.VisualEditor, "contentpage").ToString()
                + Environment.NewLine
                + TAB + _staticResourceReferenceRenderer.CssTag(_adminRouteLibrary.VisualEditor, "visualeditor").ToString()
                + Environment.NewLine
                ;
        }

        /// <summary>
        /// Generates any scripts or content that needs to be added
        /// to the end of the document body.
        /// </summary>
        /// <param name="context">
        /// The current ActionContext of the exectuing MVC action.
        /// </param>
        public async Task<string> CreateBodyScriptAsync(ActionContext context)
        {
            string responseJson = "null";

            var pageResponseData = _pageResponseDataCache.Get();
            if (pageResponseData != null)
            {
                responseJson = CreateResponseJson(pageResponseData);
            }

            var toolbarHtml = await _razorViewRenderer.RenderViewAsync(context, _adminRouteLibrary.VisualEditor.VisualEditorToolbarViewPath(), pageResponseData);
            var svgIcons = await RenderSvgIconsToStringAsync();

            var script = "<script>var Cofoundry = { 'PageResponseData': " + responseJson + " }</script>"
                + toolbarHtml
                + string.Format("<!-- SVG ICONS --><div style='{0}'>{1}</div><!-- END SVG ICONS -->", "display:none", svgIcons);

            return script;
        }
        /// <summary>
        /// Here we modify the page response data to include only what we need and
        /// serialize it into a json object.
        /// </summary>
        private string CreateResponseJson(IPageResponseData pageResponseData)
        {
            string responseJson;

            // When using IPageBlockWithParentPageData and referencing the parent page we get a
            // Self referencing loop error. Rather than set this globally we ignore this specifically here
            var settings = _jsonSerializerSettingsFactory.Create();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

            bool isCustomEntityRoute = pageResponseData.Version is CustomEntityVersionRoute;
            bool hasEntityUpdatePermission = false;
            bool hasEntityPublishPermission = false;

            if (isCustomEntityRoute)
            {
                hasEntityUpdatePermission = _permissionValidationService.HasCustomEntityPermission<CustomEntityUpdatePermission>(
                    pageResponseData.CustomEntityDefinition.CustomEntityDefinitionCode,
                    pageResponseData.CofoundryAdminUserContext
                    );
                hasEntityPublishPermission = _permissionValidationService.HasCustomEntityPermission<CustomEntityPublishPermission>(
                    pageResponseData.CustomEntityDefinition.CustomEntityDefinitionCode,
                    pageResponseData.CofoundryAdminUserContext
                    );
            }
            else
            {
                hasEntityUpdatePermission = _permissionValidationService.HasPermission<PageUpdatePermission>(pageResponseData.CofoundryAdminUserContext);
                hasEntityPublishPermission = _permissionValidationService.HasPermission<PagePublishPermission>(pageResponseData.CofoundryAdminUserContext);
            }

            var responseObject = new
            {
                Page = pageResponseData.Page,
                PageRoutingInfo = pageResponseData.PageRoutingInfo,
                PageVersion = pageResponseData.PageVersion,
                IsCustomEntityRoute = isCustomEntityRoute,
                HasDraftVersion = pageResponseData.HasDraftVersion,
                Version = pageResponseData.Version,
                VisualEditorMode = pageResponseData.VisualEditorMode,
                CustomEntityDefinition = pageResponseData.CustomEntityDefinition,
                HasEntityUpdatePermission = hasEntityUpdatePermission,
                HasEntityPublishPermission = hasEntityPublishPermission
            };

            responseJson = JsonConvert.SerializeObject(responseObject, settings);
            return responseJson;
        }

        private async Task<string> RenderSvgIconsToStringAsync()
        {
            var virtualFile = _resourceLocator.GetFile(_adminRouteLibrary.VisualEditor.StaticResourceFilePath("svg-cache.html"));
            string result = null;

            using (var stream = virtualFile.CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }
    }
}
