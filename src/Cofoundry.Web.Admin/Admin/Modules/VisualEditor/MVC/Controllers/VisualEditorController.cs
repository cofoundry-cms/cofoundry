using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Identity;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    [RouteArea(RouteConstants.AdminAreaName, AreaPrefix = RouteConstants.AdminAreaPrefix)]
    [RoutePrefix(VisualEditorRouteLibrary.RoutePrefix)]
    [Route("{action=frame}")]
    public class VisualEditorController : Controller
    {
        #region Constructors

        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserContextService _userContextService;
        private readonly IResourceLocator _resourceLocator;
        private readonly AuthenticationControllerHelper _authenticationHelper;
        private readonly AccountManagementControllerHelper _accountManagementControllerHelper;

        public VisualEditorController(
            IQueryExecutor queryExecutor,
            IUserContextService userContextService,
            IResourceLocator resourceLocator,
            AuthenticationControllerHelper authenticationHelper,
            AccountManagementControllerHelper accountManagementControllerHelper
            )
        {
            _queryExecutor = queryExecutor;
            _authenticationHelper = authenticationHelper;
            _resourceLocator = resourceLocator;
            _userContextService = userContextService;
            _accountManagementControllerHelper = accountManagementControllerHelper;
        }

        #endregion

        #region routes

        public ActionResult Frame(VisualEditorFrameModel model)
        {
            var viewPath = ViewPathFormatter.View("VisualEditor", nameof(Frame));
            return View(model);
        }

        #endregion
    }
}