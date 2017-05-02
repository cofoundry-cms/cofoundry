using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// If at this point we still do not have a valid PageRoutingInfo, we return a 404 result.
    /// This could itself be a Cofoundry page so we search for that or fallback to a ageneric 404 result.
    /// </summary>
    public interface IGetNotFoundRouteRoutingStep : IPageActionRoutingStep
    {
    }
}
