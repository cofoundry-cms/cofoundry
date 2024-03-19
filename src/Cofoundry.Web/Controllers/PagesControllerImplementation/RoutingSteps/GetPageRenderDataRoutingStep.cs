﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// the site viewer wasn't requested so we go on to gather all the data we need to render 
/// the page result. If at this point we cannot get the data (unlikely but an edge case), we 
/// return a not found result.
/// </summary>
public class GetPageRenderDataRoutingStep : IGetPageRenderDataRoutingStep
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly INotFoundViewHelper _notFoundViewHelper;

    public GetPageRenderDataRoutingStep(
        IQueryExecutor queryExecutor,
        INotFoundViewHelper notFoundViewHelper
        )
    {
        _queryExecutor = queryExecutor;
        _notFoundViewHelper = notFoundViewHelper;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        EntityInvalidOperationException.ThrowIfNull(state, state.PageRoutingInfo);
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);

        var query = new GetPageRenderDetailsByIdQuery
        {
            PageId = state.PageRoutingInfo.PageRoute.PageId
        };

        if (state.InputParameters.IsEditingCustomEntity)
        {
            // If we're editing a custom entity, then get the latest version of the page
            query.PublishStatus = PublishStatusQuery.Latest;
        }
        else if (state.InputParameters.VersionId.HasValue)
        {
            query.PublishStatus = PublishStatusQuery.SpecificVersion;
            query.PageVersionId = state.InputParameters.VersionId;
        }
        else
        {
            query.PublishStatus = state.VisualEditorState.GetPublishStatusQuery();
        }

        state.PageData = await _queryExecutor.ExecuteAsync(query);

        // if no data is found there was an issue with creating a draft earlier on.
        if (state.PageData == null && state.VisualEditorState.VisualEditorMode == VisualEditorMode.Edit)
        {
            throw new Exception("Draft version missing for page id " + query.PageId);
        }

        // If we can't find any page data, then return a 404
        if (state.PageData == null)
        {
            state.Result = await _notFoundViewHelper.GetViewAsync(controller);
        }
    }
}
