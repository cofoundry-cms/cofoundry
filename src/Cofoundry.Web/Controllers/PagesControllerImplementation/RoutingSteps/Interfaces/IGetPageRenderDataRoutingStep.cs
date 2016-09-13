using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// the site viewer wasn't requested so we go on to gather all the data we need to render 
    /// the page result. If at this point we cannot get the data (unlikely but an edge case), we 
    /// return a not found result.
    /// </summary>
    public interface IGetPageRenderDataRoutingStep : IPageActionRoutingStep
    {
    }
}
