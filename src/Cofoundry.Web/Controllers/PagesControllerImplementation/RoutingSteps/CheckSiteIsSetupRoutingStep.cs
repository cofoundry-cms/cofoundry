using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks if Cofoundry has been set up yet and if it hasn't, redirects the request
    /// to the setup screen.
    /// </summary>
    public class CheckSiteIsSetupRoutingStep : ICheckSiteIsSetupRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ISetupPageActionFactory _setupPageActionFactory;

        public CheckSiteIsSetupRoutingStep(
            IQueryExecutor queryExecutor,
            ISetupPageActionFactory setupPageActionFactory
            )
        {
            _queryExecutor = queryExecutor;
            _setupPageActionFactory = setupPageActionFactory;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            var internalSettings = _queryExecutor.Get<InternalSettings>();
            if (!internalSettings.IsSetup)
            {
                var setup = _setupPageActionFactory.GetSetupPageAction(controller);
                if (setup == null)
                {
                    throw new ApplicationException("ISetupPageActionFactory returned no action.");
                }
                state.Result = setup;
            }

            return Task.FromResult(true);
        }
    }
}
