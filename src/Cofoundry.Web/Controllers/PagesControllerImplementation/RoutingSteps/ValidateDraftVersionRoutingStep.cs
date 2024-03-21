﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cofoundry.Web;

/// <summary>
/// Checks to see if there is a valid page version for the request, if not
/// one is created.
/// </summary>
public class ValidateDraftVersionRoutingStep : IValidateDraftVersionRoutingStep
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public ValidateDraftVersionRoutingStep(
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor
        )
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);
        var pageRoutingInfo = state.PageRoutingInfo;

        if (pageRoutingInfo == null)
        {
            return;
        }

        // If there's no draft version and we're in edit mode for 'Pages' then
        // create one and re-run the query
        var publishStatus = state.VisualEditorState.GetPublishStatusQuery();
        if (!state.InputParameters.IsEditingCustomEntity
            && publishStatus != PublishStatusQuery.Published
            && publishStatus != PublishStatusQuery.SpecificVersion
            && state.IsCofoundryAdminUser)
        {
            var versionRouting = pageRoutingInfo.PageRoute.Versions.GetVersionRouting(publishStatus);
            if (versionRouting == null)
            {
                await _commandExecutor.ExecuteAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageRoutingInfo.PageRoute.PageId
                }, state.CofoundryAdminExecutionContext);

                var pageQuery = GetPageRoutingInfoQuery(state);
                pageRoutingInfo = await _queryExecutor.ExecuteAsync(pageQuery);
                EntityNotFoundException.ThrowIfNull(pageRoutingInfo);
            }
        }

        if (pageRoutingInfo.CustomEntityRoute != null
            && pageRoutingInfo.PageRoute.PageType == PageType.CustomEntityDetails)
        {
            EntityInvalidOperationException.ThrowIfNull(pageRoutingInfo, pageRoutingInfo.CustomEntityRouteRule);
            var correctUrl = pageRoutingInfo.CustomEntityRouteRule.MakeUrl(pageRoutingInfo.PageRoute, pageRoutingInfo.CustomEntityRoute);

            if (!state.InputParameters.Path.Equals(correctUrl.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Incorrect Custom Entity Url detected, redirecting from " + state.InputParameters.Path + " to " + correctUrl);
                state.Result = new RedirectResult(correctUrl, true);
            }
            else if (state.InputParameters.IsEditingCustomEntity
                && publishStatus != PublishStatusQuery.Published
                && publishStatus != PublishStatusQuery.SpecificVersion
                && state.IsCofoundryAdminUser)
            {
                var versionRouting = pageRoutingInfo.CustomEntityRoute.Versions.GetVersionRouting(publishStatus);
                if (versionRouting == null)
                {
                    // if no draft version for route, add one.
                    await _commandExecutor.ExecuteAsync(new AddCustomEntityDraftVersionCommand()
                    {
                        CustomEntityId = pageRoutingInfo.CustomEntityRoute.CustomEntityId
                    }, state.CofoundryAdminExecutionContext);

                    var pageQuery = GetPageRoutingInfoQuery(state);
                    pageRoutingInfo = await _queryExecutor.ExecuteAsync(pageQuery);
                    EntityNotFoundException.ThrowIfNull(pageRoutingInfo);
                }
            }
        }

        state.PageRoutingInfo = pageRoutingInfo;
    }

    private static GetPageRoutingInfoByPathQuery GetPageRoutingInfoQuery(PageActionRoutingState state)
    {
        ArgumentNullException.ThrowIfNull(state.VisualEditorState);

        var pageQuery = new GetPageRoutingInfoByPathQuery()
        {
            Path = state.InputParameters.Path,
            IncludeUnpublished = state.VisualEditorState.VisualEditorMode != VisualEditorMode.Live,
            LocaleId = state.Locale?.LocaleId
        };

        return pageQuery;
    }
}
