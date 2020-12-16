using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Route library for pages
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

        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        public async Task<string> PageAsync(int? pageId)
        {
            if (!pageId.HasValue) return string.Empty;

            var query = new GetPageRouteByIdQuery(pageId.Value);
            var route = await _queryExecutor.ExecuteAsync(query);

            return Page(route);
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(IPageRoute route)
        {
            if (route == null) return string.Empty;
            return route.FullPath;
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(PageRoutingInfo route)
        {
            if (route == null) return string.Empty;

            if (route.CustomEntityRouteRule != null && route.CustomEntityRoute != null)
            {
                return route.CustomEntityRouteRule.MakeUrl(route.PageRoute, route.CustomEntityRoute);
            }

            return Page(route.PageRoute);
        }

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        public string Page(ICustomEntityRoutable customEntity)
        {
            if (customEntity == null || EnumerableHelper.IsNullOrEmpty(customEntity.PageUrls)) return string.Empty;

            // Multiple details routes are technically possible, but
            // shouldn't really happen and if they are then it's reasonable
            // to expect someone to construct the routes manually themselves.
            var route = customEntity
                .PageUrls
                .OrderBy(r => r.Length)
                .FirstOrDefault();
            
            return route;
        }

        #region visual editor

        const string VISUAL_EDITOR_QS_MODE = "mode";
        const string VISUAL_EDITOR_QS_MODE_PREVIEW = "preview";
        const string VISUAL_EDITOR_QS_MODE_LIVE = "live";
        const string VISUAL_EDITOR_QS_MODE_EDIT = "edit";
        const string VISUAL_EDITOR_QS_EDITTYPE = "edittype";
        const string VISUAL_EDITOR_QS_EDITTYPE_ENTITY = "entity";
        const string VISUAL_EDITOR_QS_EDITTYPE_PAGE = "page";

        /// <summary>
        /// Gets the url for a page, formatted with specific visual editor 
        /// parameters. Note that this method does not validate permissions
        /// in any way, it simply formats the route correctly.
        /// </summary>
        /// <param name="route">The page to link to.</param>
        /// <param name="visualEditorMode">
        /// The mode to set the visual editor to. Note that this method cannot be
        /// used for VisualEditorMode.SpecificVersion and will throw an exception if
        /// you try. To get the url for a specific version, you need to use the overload
        /// accepting an IVersionRoute parameter.
        /// </param>
        /// <param name="isEditingCustomEntity">
        /// For custom entity pages, this option indicates whether the editing context 
        /// should be the custom entity rather than the (default) page.
        /// </param>
        public string VisualEditor(
            PageRoutingInfo route,
            VisualEditorMode visualEditorMode,
            bool isEditingCustomEntity = false
            )
        {
            if (route == null) return string.Empty;

            var baseUrl = Page(route);
            var queryParams = new Dictionary<string, string>(2);

            switch (visualEditorMode)
            {
                case VisualEditorMode.Any:

                    var latestVersion = route.GetVersionRoute(isEditingCustomEntity, PublishStatusQuery.Latest, null);

                    if (latestVersion.WorkFlowStatus == WorkFlowStatus.Draft || !route.IsPublished())
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

        /// <summary>
        /// Gets the url for a page at a specific page or custom entity version, loaded inside the 
        /// visual editor. Note that this method does not validate permissions in any way, it simply 
        /// formats the route correctly.
        /// </summary>
        /// <param name="route">The page to link to.</param>
        /// <param name="versionRoute">The version of the page or custom entity to link to.</param>
        public string VisualEditor(
            PageRoutingInfo route,
            IVersionRoute versionRoute
            )
        {
            if (route == null) return string.Empty;

            if (versionRoute == null)
            {
                throw new ArgumentNullException(nameof(versionRoute));
            }

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
            var queryParams = new Dictionary<string, string>(2);
            queryParams.Add("version", versionRoute.VersionId.ToString());

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
}
