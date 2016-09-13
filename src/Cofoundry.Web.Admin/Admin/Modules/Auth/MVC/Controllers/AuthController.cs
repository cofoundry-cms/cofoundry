using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Identity;
using Cofoundry.Domain.MailTemplates;

namespace Cofoundry.Web.Admin
{
    [RouteArea(RouteConstants.AdminAreaName, AreaPrefix = RouteConstants.AdminAreaPrefix)]
    [RoutePrefix(AuthRouteLibrary.RoutePrefix)]
    [Route("{action=login}")]
    public class AuthController : Controller
    {
        #region Constructors

        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserContextService _userContextService;
        private readonly AuthenticationControllerHelper _authenticationHelper;
        private readonly AccountManagementControllerHelper _accountManagementControllerHelper;

        public AuthController(
            IQueryExecutor queryExecutor,
            IUserContextService userContextService,
            AuthenticationControllerHelper authenticationHelper,
            AccountManagementControllerHelper accountManagementControllerHelper
            )
        {
            _queryExecutor = queryExecutor;
            _authenticationHelper = authenticationHelper;
            _userContextService = userContextService;
            _accountManagementControllerHelper = accountManagementControllerHelper;
        }

        #endregion

        #region OnActionExecuting

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var settings = _queryExecutor.Get<InternalSettings>();
            if (!settings.IsSetup)
            {
                filterContext.Result = Redirect(SetupRouteLibrary.Urls.Setup());
            }
        }

        #endregion

        #region routes

        [Route("~" + RouteConstants.AdminUrlRoot)]
        public ActionResult DefaultRedirect()
        {
            return RedirectPermanent(AuthRouteLibrary.Urls.Login());
        }

        public ActionResult Login(string email)
        {
            var user = _userContextService.GetCurrentContext();
            if (user.IsCofoundryUser()) return GetLoggedInDefaultRedirectAction();
            return View(new LoginViewModel { EmailAddress = email });
        }

        [HttpPost]
        public ActionResult Login(string returnUrl, LoginViewModel command)
        {
            var result = _authenticationHelper.LogUserIn(this, command, new CofoundryAdminUserArea());

            if (result.IsAuthenticated && result.RequiresPasswordChange)
            {
                return Redirect(AuthRouteLibrary.Urls.ChangePassword(returnUrl));
            }
            else if (result.IsAuthenticated && !string.IsNullOrEmpty(result.ReturnUrl))
            {
                return Redirect(result.ReturnUrl);
            }
            else if (result.IsAuthenticated)
            {
                _userContextService.ClearCache();
                return GetLoggedInDefaultRedirectAction();
            }

            return View(command);
        }

        public ActionResult Logout()
        {
            _authenticationHelper.Logout();
            return Redirect(AuthRouteLibrary.Urls.Login(Request.QueryString["ReturnUrl"]));
        }

        [Route("forgot-password")]
        public ActionResult ForgotPassword(string email)
        {
            var user = _userContextService.GetCurrentContext();
            if (user.IsCofoundryUser()) return GetLoggedInDefaultRedirectAction();

            return View(new ForgotPasswordViewModel { Username = email });
        }

        [Route("forgot-password")]
        [HttpPost]
        public ViewResult ForgotPassword(ForgotPasswordViewModel command)
        {
            var template = new ResetPasswordTemplate();
            _authenticationHelper.SendPasswordResetNotification(this, command, template, new CofoundryAdminUserArea());

            return View(command);
        }

        [Route("reset-password")]
        public ActionResult ResetPassword(string i, string t)
        {
            var user = _userContextService.GetCurrentContext();
            if (user.IsCofoundryUser()) return GetLoggedInDefaultRedirectAction();

            var request = _authenticationHelper.IsPasswordRequestValid(this, i, t, new CofoundryAdminUserArea());

            if (!request.IsValid) return View("ResetPasswordRequestInvalid", request);

            var vm = new CompletePasswordResetViewModel();
            vm.UserPasswordResetRequestId = i;
            vm.Token = t;

            return View(vm);
        }

        [Route("reset-password")]
        [HttpPost]
        public ActionResult ResetPassword(CompletePasswordResetViewModel vm)
        {
            var user = _userContextService.GetCurrentContext();
            if (user.IsCofoundryUser()) return GetLoggedInDefaultRedirectAction();

            _authenticationHelper.CompletePasswordReset(this, vm, new PasswordChangedTemplate(), new CofoundryAdminUserArea());
            
            return Redirect(AuthRouteLibrary.Urls.Login());
        }


        [AdminAuthorize]
        [Route("change-password")]
        public ActionResult ChangePassword(string returnUrl)
        {
            var user = _userContextService.GetCurrentContext();
            if (!user.IsPasswordChangeRequired) return GetLoggedInDefaultRedirectAction();

            return View(new ChangePasswordViewModel());
        }

        [AdminAuthorize]
        [Route("change-password")]
        [HttpPost]
        public ActionResult ChangePassword(string returnUrl, ChangePasswordViewModel vm)
        {
            var user = _userContextService.GetCurrentContext();
            if (!user.IsPasswordChangeRequired) return GetLoggedInDefaultRedirectAction();

            _accountManagementControllerHelper.ChangePassword(this, vm);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return GetLoggedInDefaultRedirectAction();
                }
            }

            return View(vm);
        }

        #endregion

        #region helpers

        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            var modules = _queryExecutor.Execute(new GetPermittedAdminModulesQuery());
            var dashboardModule = modules.FirstOrDefault(m => m.AdminModuleCode == DashboardModuleRegistration.ModuleCode);

            if (dashboardModule == null)
            {
                dashboardModule = modules
                    .SetStandardOrdering()
                    .FirstOrDefault();
            }

            if (dashboardModule == null)
            {
                throw new InvalidOperationException("No modules available.");
            }

            return Redirect(dashboardModule.Url);
        }

        #endregion
    }
}