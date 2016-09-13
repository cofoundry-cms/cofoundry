using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// The default implementation returns no site viewer. The real implementation is
    /// in Cofoundry.Web.Admin.SiteViewerActionFactory
    /// </summary>
    public class SetupPageActionFactory : ISetupPageActionFactory
    {
        private readonly IQueryExecutor _queryExecutor;

        public SetupPageActionFactory(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public ActionResult GetSetupPageAction(Controller controller)
        {
            return new RedirectResult(SetupRouteLibrary.Urls.Setup());
        }
    }
}