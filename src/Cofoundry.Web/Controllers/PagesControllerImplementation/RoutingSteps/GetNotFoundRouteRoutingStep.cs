using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// If at this point we still do not have a valid PageRoutingInfo, we return a 404 result.
/// This could itself be a Cofoundry Page so we search for that or fallback to a ageneric 404 result.
/// </summary>
public class GetNotFoundRouteRoutingStep : IGetNotFoundRouteRoutingStep
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly INotFoundViewHelper _notFoundViewHelper;

    public GetNotFoundRouteRoutingStep(
        IQueryExecutor queryExecutor,
        INotFoundViewHelper notFoundViewHelper
        )
    {
        _queryExecutor = queryExecutor;
        _notFoundViewHelper = notFoundViewHelper;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);

        // Find a 404 page if a version does not exist.
        if (state.PageRoutingInfo == null)
        {
            // First check for a rewrite rule and apply it
            state.Result = await GetRewriteResult(controller);
            if (state.Result != null)
            {
                return;
            }

            // else try and find a 404 page route
            state.PageRoutingInfo = await TryFindNotFoundPageRoute(state.InputParameters.Path, state.VisualEditorState.VisualEditorMode);

            // If we still can't find a 404, fall back to the generic 404 view
            if (state.PageRoutingInfo == null)
            {
                state.Result = await _notFoundViewHelper.GetViewAsync(controller);
            }
        }
    }

    private async Task<ActionResult?> GetRewriteResult(Controller controller)
    {
        var rewriteRule = await _queryExecutor.ExecuteAsync(new GetRewriteRuleSummaryByPathQuery()
        {
            Path = controller.Request.Path
        });

        if (rewriteRule != null)
        {
            var response = new RedirectResult(rewriteRule.WriteTo, true);
            return response;
        }

        return null;
    }

    /// <summary>
    /// Try and find a page route for a 404 page.
    /// </summary>
    private async Task<PageRoutingInfo?> TryFindNotFoundPageRoute(string path, VisualEditorMode siteViewerMode)
    {
        var pageRoute = await _queryExecutor.ExecuteAsync(new GetNotFoundPageRouteByPathQuery()
        {
            Path = path,
            IncludeUnpublished = siteViewerMode != VisualEditorMode.Live
        });

        if (pageRoute == null)
        {
            return null;
        }

        return new PageRoutingInfo()
        {
            PageRoute = pageRoute
        };
    }
}
