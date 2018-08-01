using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
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
            var pageQuery = GetPageRoutingInfoQuery(state);
            var pageRoutingInfo = await _queryExecutor.ExecuteAsync(pageQuery);
            state.PageRoutingInfo = pageRoutingInfo;
        }

        private GetPageRoutingInfoByPathQuery GetPageRoutingInfoQuery(PageActionRoutingState state)
        {
            var pageQuery = new GetPageRoutingInfoByPathQuery()
            {
                Path = state.InputParameters.Path,
                IncludeUnpublished = state.VisualEditorState.VisualEditorMode != VisualEditorMode.Live
            };

            if (state.Locale != null)
            {
                pageQuery.LocaleId = state.Locale.LocaleId;
            }

            return pageQuery;
        }
    }
}
