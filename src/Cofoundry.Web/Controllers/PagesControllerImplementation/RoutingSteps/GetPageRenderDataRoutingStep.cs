using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
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
            var query = new GetPageRenderDetailsByIdQuery();
            query.PageId = state.PageRoutingInfo.PageRoute.PageId;

            // If we're editing a custom entity, then get the latest version
            if (!state.InputParameters.IsEditingCustomEntity)
            {
                query.WorkFlowStatus = state.SiteViewerMode.ToWorkFlowStatusQuery();
                query.PageVersionId = state.InputParameters.VersionId;
            }

            state.PageData = await _queryExecutor.ExecuteAsync(query);

            // if no data is found there was an issue with creating a draft earlier on.
            if (state.PageData == null && state.SiteViewerMode == SiteViewerMode.Edit)
            {
                throw new ApplicationException("Draft version missing for page id " + query.PageId);
            }

            // If we can't find any page data, then return a 404
            if (state.PageData == null)
            {
                state.Result = _notFoundViewHelper.GetView();
            }
        }
    }
}
