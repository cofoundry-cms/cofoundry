using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if there is a valid page version for the request, if not
    /// one is created.
    /// </summary>
    public interface IValidateDraftVersionRoutingStep : IPageActionRoutingStep
    {
    }
}
