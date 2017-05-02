using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cofoundry.Web
{
    /// <summary>
    /// The default implementation returns no site viewer. The real implementation is
    /// in Cofoundry.Web.Admin.SiteViewerActionFactory
    /// </summary>
    public class DefaultSetupPageActionFactory : ISetupPageActionFactory
    {
        private readonly IQueryExecutor _queryExecutor;

        public DefaultSetupPageActionFactory(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public ActionResult GetSetupPageAction(Controller controller)
        {
            throw new ApplicationException("Cofoundry has not been setup. Admin panel is not installed so installation must be done manually.");
        }
    }
}