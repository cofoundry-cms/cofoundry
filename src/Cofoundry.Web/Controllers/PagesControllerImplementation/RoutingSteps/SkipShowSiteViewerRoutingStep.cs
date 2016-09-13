using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// The default implementation skips the siteviewer step, because it is only to be used
    /// when the admin package is included. See Cofoundry.Web.Admin.ShowSiteViewerRoutingStep
    /// </summary>
    public class SkipShowSiteViewerRoutingStep : IShowSiteViewerRoutingStep
    {
        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            return Task.FromResult(true);
        }
    }
}
