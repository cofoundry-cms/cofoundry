using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// Attempts to set the PageActionRoutingState.PageRoutingInfo property by querying
/// for an exact match to the request
/// </summary>
public class TryFindPageRoutingInfoRoutingStep : ITryFindPageRoutingInfoRoutingStep
{
    private readonly IQueryExecutor _queryExecutor;

    public TryFindPageRoutingInfoRoutingStep(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);

        var pageRoutingInfo = await _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByPathQuery()
        {
            Path = state.InputParameters.Path,
            IncludeUnpublished = state.VisualEditorState.VisualEditorMode != VisualEditorMode.Live,
            LocaleId = state.Locale?.LocaleId
        });

        state.PageRoutingInfo = pageRoutingInfo;
    }
}
