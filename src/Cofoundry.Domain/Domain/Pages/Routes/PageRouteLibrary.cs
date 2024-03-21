using Microsoft.AspNetCore.WebUtilities;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRouteLibrary"/>.
/// </summary>
public class PageRouteLibrary : IPageRouteLibrary
{
    private readonly IQueryExecutor _queryExecutor;

    public PageRouteLibrary(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    /// <inheritdoc/>
    public async Task<string> PageAsync(int? pageId)
    {
        if (!pageId.HasValue)
        {
            return string.Empty;
        }

        var query = new GetPageRouteByIdQuery(pageId.Value);
        var route = await _queryExecutor.ExecuteAsync(query);

        return Page(route);
    }

    /// <inheritdoc/>
    public string Page(IPageRoute? route)
    {
        if (route == null)
        {
            return string.Empty;
        }

        return route.FullUrlPath;
    }

    /// <inheritdoc/>
    public string Page(PageRoutingInfo? route)
    {
        if (route == null)
        {
            return string.Empty;
        }

        if (route.CustomEntityRouteRule != null && route.CustomEntityRoute != null)
        {
            return route.CustomEntityRouteRule.MakeUrl(route.PageRoute, route.CustomEntityRoute);
        }

        return Page(route.PageRoute);
    }

    /// <inheritdoc/>
    public string Page(ICustomEntityRoutable? customEntity)
    {
        if (customEntity == null || EnumerableHelper.IsNullOrEmpty(customEntity.PageUrls))
        {
            return string.Empty;
        }

        // Multiple details routes are technically possible, but
        // shouldn't really happen and if they are then it's reasonable
        // to expect someone to construct the routes manually themselves.
        var route = customEntity
            .PageUrls
            .OrderBy(r => r.Length)
            .FirstOrDefault();

        return route ?? string.Empty;
    }

    #region visual editor

    const string VISUAL_EDITOR_QS_MODE = "mode";
    const string VISUAL_EDITOR_QS_MODE_PREVIEW = "preview";
    const string VISUAL_EDITOR_QS_MODE_LIVE = "live";
    const string VISUAL_EDITOR_QS_MODE_EDIT = "edit";
    const string VISUAL_EDITOR_QS_EDITTYPE = "edittype";
    const string VISUAL_EDITOR_QS_EDITTYPE_ENTITY = "entity";
    const string VISUAL_EDITOR_QS_EDITTYPE_PAGE = "page";

    /// <inheritdoc/>
    public string VisualEditor(
        PageRoutingInfo? route,
        VisualEditorMode visualEditorMode,
        bool isEditingCustomEntity = false
        )
    {
        if (route == null)
        {
            return string.Empty;
        }

        var baseUrl = Page(route);
        var queryParams = new Dictionary<string, string?>(2);

        switch (visualEditorMode)
        {
            case VisualEditorMode.Any:

                var latestVersion = route.GetVersionRoute(isEditingCustomEntity, PublishStatusQuery.Latest, null);

                if (latestVersion != null && latestVersion.WorkFlowStatus == WorkFlowStatus.Draft || !route.IsPublished())
                {
                    queryParams.Add(VISUAL_EDITOR_QS_MODE, VISUAL_EDITOR_QS_MODE_PREVIEW);
                }
                else
                {
                    queryParams.Add(VISUAL_EDITOR_QS_MODE, VISUAL_EDITOR_QS_MODE_LIVE);
                }
                break;
            case VisualEditorMode.Preview:
                queryParams.Add(VISUAL_EDITOR_QS_MODE, VISUAL_EDITOR_QS_MODE_PREVIEW);
                break;
            case VisualEditorMode.Edit:
                queryParams.Add(VISUAL_EDITOR_QS_MODE, VISUAL_EDITOR_QS_MODE_EDIT);
                break;
            case VisualEditorMode.Live:
                queryParams.Add(VISUAL_EDITOR_QS_MODE, VISUAL_EDITOR_QS_MODE_LIVE);
                break;
            case VisualEditorMode.SpecificVersion:
                throw new Exception($"To create a url for {nameof(VisualEditorMode)}.{nameof(VisualEditorMode.SpecificVersion)} you should use the overload that takes an {nameof(IVersionRoute)} parameter");
            default:
                throw new Exception($"{nameof(VisualEditorMode)}.{visualEditorMode} not supported by {nameof(PageRouteLibrary)}.{nameof(VisualEditor)}()");
        }

        if (isEditingCustomEntity)
        {
            queryParams.Add(VISUAL_EDITOR_QS_EDITTYPE, VISUAL_EDITOR_QS_EDITTYPE_ENTITY);
        }
        else if (route.CustomEntityRoute != null)
        {
            queryParams.Add(VISUAL_EDITOR_QS_EDITTYPE, VISUAL_EDITOR_QS_EDITTYPE_PAGE);
        }

        var url = QueryHelpers.AddQueryString(baseUrl, queryParams);
        return url;
    }

    /// <inheritdoc/>
    public string VisualEditor(
        PageRoutingInfo? route,
        IVersionRoute versionRoute
        )
    {
        if (route == null)
        {
            return string.Empty;
        }

        ArgumentNullException.ThrowIfNull(versionRoute);

        var isEditingCustomEntity = versionRoute is CustomEntityVersionRoute;

        // Some of the latest version states will have a default view e.g. preview 
        // or live so check for these first before we defer to showing by version number

        if (versionRoute.IsLatestPublishedVersion && route.IsPublished())
        {
            // Published, so show live view
            return VisualEditor(route, VisualEditorMode.Live, isEditingCustomEntity);
        }
        else if (versionRoute.WorkFlowStatus == WorkFlowStatus.Draft)
        {
            // Draft version, so show draft
            return VisualEditor(route, VisualEditorMode.Preview, isEditingCustomEntity);
        }

        var baseUrl = Page(route);
        var queryParams = new Dictionary<string, string?>(2)
        {
            { "version", versionRoute.VersionId.ToString(CultureInfo.InvariantCulture) }
        };

        if (isEditingCustomEntity)
        {
            queryParams.Add(VISUAL_EDITOR_QS_EDITTYPE, VISUAL_EDITOR_QS_EDITTYPE_ENTITY);
        }
        else if (route.CustomEntityRoute != null)
        {
            queryParams.Add(VISUAL_EDITOR_QS_EDITTYPE, VISUAL_EDITOR_QS_EDITTYPE_PAGE);
        }

        var url = QueryHelpers.AddQueryString(baseUrl, queryParams);
        return url;
    }

    #endregion
}
