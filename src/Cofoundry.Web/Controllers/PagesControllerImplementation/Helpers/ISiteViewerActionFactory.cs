using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory to detach the implementation of the site viewer page, which is handled in Cofoundry.Web.Admin
    /// </summary>
    public interface ISiteViewerActionFactory
    {
        /// <summary>
        /// Gets a site viewer view result conrresponding the specified PageActionRoutingState. The
        /// PageActionRoutingState can be null or have no PageRoute property, in which case a non-page specific
        /// site viewer is returned. If no site viewer is to be rendered then null will be returned.
        /// </summary>
        ActionResult GetSiteViewerAction(Controller controller, PageActionRoutingState state = null);
    }
}