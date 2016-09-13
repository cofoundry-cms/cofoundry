using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// If the site viewer has been requested, we now have all the information we need to 
    /// display it, so here we construct the site viewer result.
    /// </summary>
    public interface IShowSiteViewerRoutingStep : IPageActionRoutingStep
    {
    }
}
