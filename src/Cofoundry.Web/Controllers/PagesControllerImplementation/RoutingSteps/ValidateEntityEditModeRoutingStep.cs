using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Validates the PageActionRoutingState.IsEditingCustomEntity property. We set 
    /// PageActionRoutingState.IsEditingCustomEntity to true by default because we want to prefer
    /// entity editing when it is available over page editing, so here we check to see
    /// if setting IsEditingCustomEntity to true is invalid and revert it.
    /// </summary>
    public class ValidateEntityEditModeRoutingStep : IValidateEntityEditModeRoutingStep
    {
        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            var completedTask = Task.FromResult(true);

            var pageRoutingInfo = state.PageRoutingInfo;
            if (pageRoutingInfo == null) return completedTask;

            if (pageRoutingInfo.CustomEntityRoute == null && state.InputParameters.IsEditingCustomEntity)
            {
                state.InputParameters.IsEditingCustomEntity = false;
            }

            return completedTask;
        }
    }
}
