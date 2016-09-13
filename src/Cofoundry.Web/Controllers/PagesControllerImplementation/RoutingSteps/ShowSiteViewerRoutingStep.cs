using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Conditions;

namespace Cofoundry.Web
{
    /// <summary>
    /// If the site viewer has been requested, we now have all the information we need to 
    /// display it, so here we construct the site viewer result.
    /// </summary>
    public class ShowSiteViewerRoutingStep : IShowSiteViewerRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ISiteViewerActionFactory _siteViewerActionFactory;

        public ShowSiteViewerRoutingStep(
            IQueryExecutor queryExecutor,
            ISiteViewerActionFactory siteViewerActionFactory
            )
        {
            _queryExecutor = queryExecutor;
            _siteViewerActionFactory = siteViewerActionFactory;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            if (state.InputParameters.IsSiteViewerRequested && state.UserContext.IsCofoundryUser())
            {
                state.Result = _siteViewerActionFactory.GetSiteViewerAction(controller, state);
            }

            return Task.FromResult(true);
        }
    }
}
