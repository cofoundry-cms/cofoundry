using Cofoundry.Core.Json;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cofoundry.Web.Admin.Internal;

/// <summary>
/// Default implementation of <see cref="IVisualEditorScriptGenerator"/>.
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

    /// <inheritdoc/>
    public string CreateHeadScript()
    {
        return TAB + _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.VisualEditor, "contentpage").ToString()
            + Environment.NewLine
            + TAB + _staticResourceReferenceRenderer.CssTag(_adminRouteLibrary.VisualEditor, "visualeditor").ToString()
            + Environment.NewLine
            ;
    }

    /// <inheritdoc/>
    public async Task<string> CreateBodyScriptAsync(ActionContext context)
    {
        var responseJson = "null";

        var pageResponseData = _pageResponseDataCache.Get();
        if (pageResponseData != null)
        {
            responseJson = CreateResponseJson(pageResponseData);
        }

        var toolbarHtml = await _razorViewRenderer.RenderViewAsync(context, _adminRouteLibrary.VisualEditor.VisualEditorToolbarViewPath(), pageResponseData);
        var svgIcons = await RenderSvgIconsToStringAsync();

        var script = "<script>var Cofoundry = { 'PageResponseData': " + responseJson + " }</script>"
            + toolbarHtml
            + $"<!-- SVG ICONS --><div style='display:none'>{svgIcons}</div><!-- END SVG ICONS -->";

        return script;
    }
    /// <summary>
    /// Here we modify the page response data to include only what we need and
    /// serialize it into a json object.
    /// </summary>
    private string CreateResponseJson(IPageResponseData pageResponseData)
    {
        EntityInvalidOperationException.ThrowIfNull(pageResponseData, pageResponseData.CofoundryAdminUserContext);

        string responseJson;

        // When using IPageBlockWithParentPageData and referencing the parent page we get a
        // Self referencing loop error. Rather than set this globally we ignore this specifically here
        var settings = _jsonSerializerSettingsFactory.Create();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

        var isCustomEntityRoute = pageResponseData.Version is CustomEntityVersionRoute;
        bool hasEntityUpdatePermission;
        bool hasEntityPublishPermission;

        if (isCustomEntityRoute)
        {
            EntityInvalidOperationException.ThrowIfNull(pageResponseData, pageResponseData.CustomEntityDefinition);

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
            pageResponseData.Page,
            pageResponseData.PageRoutingInfo,
            pageResponseData.PageVersion,
            pageResponseData.HasDraftVersion,
            pageResponseData.Version,
            pageResponseData.VisualEditorMode,
            pageResponseData.CustomEntityDefinition,
            IsCustomEntityRoute = isCustomEntityRoute,
            HasEntityUpdatePermission = hasEntityUpdatePermission,
            HasEntityPublishPermission = hasEntityPublishPermission
        };

        responseJson = JsonConvert.SerializeObject(responseObject, settings);
        return responseJson;
    }

    private async Task<string> RenderSvgIconsToStringAsync()
    {
        var virtualFile = _resourceLocator.GetFile(_adminRouteLibrary.VisualEditor.StaticResourceFilePath("svg-cache.html"));

        using var stream = virtualFile.CreateReadStream();
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();

        return result;
    }
}
