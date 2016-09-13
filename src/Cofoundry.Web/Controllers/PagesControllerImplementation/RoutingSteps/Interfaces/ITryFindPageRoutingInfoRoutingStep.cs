using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Attempts to set the PageActionRoutingState.PageRoutingInfo property by querying
    /// for an exact match to the request
    /// </summary>
    public interface ITryFindPageRoutingInfoRoutingStep : IPageActionRoutingStep
    {
    }
}
