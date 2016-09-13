using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// The default implementation returns no site viewer. The real implementation is
    /// in Cofoundry.Web.Admin.SiteViewerActionFactory
    /// </summary>
    public class DefaultSiteViewerActionFactory : ISiteViewerActionFactory
    {
        private readonly IQueryExecutor _queryExecutor;

        public DefaultSiteViewerActionFactory(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public ActionResult GetSiteViewerAction(Controller controller, PageActionRoutingState state = null)
        {
            return null;
        }
    }
}