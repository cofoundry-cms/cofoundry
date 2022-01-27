using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    [AdminAuthorize]
    public class AuthController : Controller
    {
        private static readonly string CONTROLLER_NAME = "Auth";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserContextService _userContextService;
        private readonly IAuthenticationControllerHelper<CofoundryAdminUserArea> _authenticationControllerHelper;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AuthController(
            IQueryExecutor queryExecutor,
            IUserContextService userContextService,
            IAuthenticationControllerHelper<CofoundryAdminUserArea> authenticationControllerHelper,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _authenticationControllerHelper = authenticationControllerHelper;
            _userContextService = userContextService;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<InternalSettings>());
            if (!settings.IsSetup)
            {
                context.Result = Redirect(_adminRouteLibrary.Setup.Setup());
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }

        [AllowAnonymous]
        public ActionResult DefaultRedirect()
        {
            return RedirectPermanent(_adminRouteLibrary.Auth.Login());
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
            var vm = new LoginViewModel();

            return View(viewPath, vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string returnUrl, LoginViewModel viewModel)
        {
            var authResult = await _authenticationControllerHelper.AuthenticateAsync(this, viewModel);

            if (!authResult.IsSuccess)
            {
                var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
                return View(viewPath, viewModel);
            }

            // Support redirect urls from login
            var redirectUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            if (authResult.User.RequirePasswordChange)
            {
                return Redirect(_adminRouteLibrary.Auth.ChangePassword(returnUrl));
            }

            // If no action required, log the user in
            await _authenticationControllerHelper.LogUserInAsync(this, authResult.User, true);

            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return await GetLoggedInDefaultRedirectActionAsync();
        }

        public async Task<ActionResult> Logout()
        {
            await _authenticationControllerHelper.LogoutAsync();
            return Redirect(_adminRouteLibrary.Auth.Login());
        }

        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, new ForgotPasswordViewModel { Username = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            await _authenticationControllerHelper.SendAccountRecoveryNotificationAsync(this, command);

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, command);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword()
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            var requestValidationResult = await _authenticationControllerHelper.ParseAndValidateAccountRecoveryRequestAsync(this);

            if (!requestValidationResult.IsSuccess)
            {
                var invalidViewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword) + "RequestInvalid");
                return View(invalidViewPath, requestValidationResult);
            }

            var vm = new CompleteAccountRecoveryViewModel(requestValidationResult);
            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword));

            return View(viewPath, vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(CompleteAccountRecoveryViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.IsCofoundryUser()) return await GetLoggedInDefaultRedirectActionAsync();

            await _authenticationControllerHelper.CompleteAccountRecoveryAsync(this, vm);

            if (ModelState.IsValid)
            {
                var completeViewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword) + "Complete");
                return View(completeViewPath);
            }

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword));
            return View(viewPath, vm);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword(string returnUrl)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.UserId.HasValue)
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return await GetLoggedInDefaultRedirectActionAsync();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync();
            }

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, new ChangePasswordViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string returnUrl, ChangePasswordViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextAsync();
            if (user.UserId.HasValue)
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return await GetLoggedInDefaultRedirectActionAsync();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync();
            }

            await _authenticationControllerHelper.ChangePasswordAsync(this, vm);

            ViewBag.ReturnUrl = returnUrl;

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, vm);
        }

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
    }
}