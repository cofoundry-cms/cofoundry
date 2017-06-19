using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Identity;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    [AdminRoute(AuthRouteLibrary.RoutePrefix)]
    public class AuthController : Controller
    {
        private static readonly string CONTROLLER_NAME = "Auth";

        #region Constructors

        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserContextService _userContextService;
        private readonly AuthenticationControllerHelper _authenticationHelper;
        private readonly AccountManagementControllerHelper _accountManagementControllerHelper;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AuthController(
            IQueryExecutor queryExecutor,
            IUserContextService userContextService,
            AuthenticationControllerHelper authenticationHelper,
            AccountManagementControllerHelper accountManagementControllerHelper,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _authenticationHelper = authenticationHelper;
            _userContextService = userContextService;
            _accountManagementControllerHelper = accountManagementControllerHelper;
            _adminRouteLibrary = adminRouteLibrary;
        }

        #endregion

        #region OnActionExecuting

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = await _queryExecutor.GetAsync<InternalSettings>();
            if (!settings.IsSetup)
            {
                context.Result = Redirect(_adminRouteLibrary.Setup.Setup());
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }

        #endregion

        #region routes

        [Route("")]
        public ActionResult Index(string email)
        {
            return RedirectToActionPermanent(nameof(Login), new { email = email });
        }

        [Route("login")]
        public async Task<ActionResult> Login(string email)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
            var vm = new LoginViewModel { EmailAddress = email };

            return View(viewPath, vm);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string returnUrl, LoginViewModel command)
        {
            var result = await _authenticationHelper.LogUserInAsync(this, command, new CofoundryAdminUserArea());

            if (result.IsAuthenticated && result.RequiresPasswordChange)
            {
                return Redirect(_adminRouteLibrary.Auth.ChangePassword(returnUrl));
            }
            else if (result.IsAuthenticated && !string.IsNullOrEmpty(result.ReturnUrl))
            {
                return Redirect(result.ReturnUrl);
            }
            else if (result.IsAuthenticated)
            {
                _userContextService.ClearCache();
                return await GetLoggedInDefaultRedirectActionAsync();
            }

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
            return View(viewPath, command);
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authenticationHelper.LogoutAsync();
            return Redirect(_adminRouteLibrary.Auth.Login(Request.Query["ReturnUrl"].FirstOrDefault()));
        }

        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, new ForgotPasswordViewModel { Username = email });
        }

        [HttpPost("forgot-password")]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            var template = new ResetPasswordTemplate();
            await _authenticationHelper.SendPasswordResetNotificationAsync(this, command, template, new CofoundryAdminUserArea());

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, command);
        }

        [Route("reset-password")]
        public async Task<ActionResult> ResetPassword(string i, string t)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var request = await _authenticationHelper.IsPasswordRequestValidAsync(this, i, t, new CofoundryAdminUserArea());

            if (!request.IsValid) return View("ResetPasswordRequestInvalid", request);

            var vm = new CompletePasswordResetViewModel();
            vm.UserPasswordResetRequestId = i;
            vm.Token = t;

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword));
            return View(viewPath, vm);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(CompletePasswordResetViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            await _authenticationHelper.CompletePasswordResetAsync(this, vm, new PasswordChangedTemplate(), new CofoundryAdminUserArea());
            
            return Redirect(_adminRouteLibrary.Auth.Login());
        }


        [AdminAuthorize]
        [Route("change-password")]
        public async Task<ActionResult> ChangePassword(string returnUrl)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (!user.IsPasswordChangeRequired) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, new ChangePasswordViewModel());
        }

        [AdminAuthorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(string returnUrl, ChangePasswordViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (!user.IsPasswordChangeRequired) return await GetLoggedInDefaultRedirectActionAsync();

            await _accountManagementControllerHelper.ChangePasswordAsync(this, vm);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return await GetLoggedInDefaultRedirectActionAsync();
                }
            }

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, vm);
        }

        #endregion

        #region helpers

        private async Task<ActionResult> GetLoggedInDefaultRedirectActionAsync()
        {
            var modules = await _queryExecutor.ExecuteAsync(new GetPermittedAdminModulesQuery());
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