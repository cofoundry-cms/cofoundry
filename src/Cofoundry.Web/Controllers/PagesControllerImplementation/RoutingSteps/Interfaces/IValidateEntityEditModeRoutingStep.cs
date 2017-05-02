using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Validates the PageActionRoutingState.IsEditingCustomEntity property. We set 
    /// PageActionRoutingState.IsEditingCustomEntity to true by default because we want to prefer
    /// entity editing when it is available over page editing, so here we check to see
    /// if setting IsEditingCustomEntity to true is invalid and revert it.
    /// </summary>
    public interface IValidateEntityEditModeRoutingStep : IPageActionRoutingStep
    {
    }
}
