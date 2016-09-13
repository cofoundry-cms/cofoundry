using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Here we set a cache policy for the result before it goes out.
    /// </summary>
    public interface ISetCachePolicyRoutingStep : IPageActionRoutingStep
    {
    }
}
