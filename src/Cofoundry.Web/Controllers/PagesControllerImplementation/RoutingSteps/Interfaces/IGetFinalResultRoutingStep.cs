using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// In this last step we construct the view models and view result for the page. Some special page types
    /// have further actions applied (e.g. custom entity details pages).
    /// </summary>
    public interface IGetFinalResultRoutingStep : IPageActionRoutingStep
    {
    }
}
