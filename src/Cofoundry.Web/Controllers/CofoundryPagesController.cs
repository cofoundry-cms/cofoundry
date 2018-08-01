using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main controller for handling Cofoundry page routing, redirection and not found errors. This route
    /// is configured last so all other controller routes are scanned first before falling back to this. 
    /// </summary>
    public class CofoundryPagesController : Controller
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageActionRoutingStepFactory _pageActionRoutingStepFactory;

        public CofoundryPagesController(
            IQueryExecutor queryExecutor,
            IPageActionRoutingStepFactory pageActionRoutingStepFactory
            )
        {
            _queryExecutor = queryExecutor;
            _pageActionRoutingStepFactory = pageActionRoutingStepFactory;
            
        }

        #endregion

        public async Task<IActionResult> Page(
            string path, 
            string mode, 
            int? version = null,
            string editType = "entity"
            )
        {
            // Init state
            var state = new PageActionRoutingState();
            state.Locale = await _queryExecutor.ExecuteAsync(new GetCurrentActiveLocaleQuery());
            state.InputParameters = new PageActionInputParameters()
            {
                Path = path,
                VersionId = version,
                IsEditingCustomEntity = editType == "entity"
            };

            // Run through the pipline in order
            foreach (var method in _pageActionRoutingStepFactory.Create())
            {
                await method.ExecuteAsync(this, state);
                // If we get an action result, do an early return
                if (state.Result != null)
                {
                    return state.Result;
                }
            }

            // We should never get here!
            throw new InvalidOperationException("Unknown Page Routing State");
        }
    }
}