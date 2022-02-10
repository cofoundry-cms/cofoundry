using Cofoundry.Domain;
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

        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public AuthController(
            IAdvancedContentRepository contentRepository,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _contentRepository = contentRepository.WithContext<CofoundryAdminUserArea>();
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = await _contentRepository.ExecuteQueryAsync(new GetSettingsQuery<InternalSettings>());

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
            if (await IsLoggedInAsync()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
            var vm = new SignInViewModel();

            return View(viewPath, vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string returnUrl, SignInViewModel viewModel)
        {
            var authResult = await _contentRepository
                .WithModelState(this)
                .Users()
                .Authentication()
                .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
                {
                    Username = viewModel.Username,
                    Password = viewModel.Password,
                    UserAreaCode = CofoundryAdminUserArea.Code
                })
                .ExecuteAsync();

            if (!ModelState.IsValid)
            {
                var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(Login));
                return View(viewPath, viewModel);
            }

            // Support redirect urls from login
            var redirectUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

            if (authResult.User.RequirePasswordChange)
            {
                return Redirect(_adminRouteLibrary.Auth.ChangePassword(returnUrl));
            }

            // If no action required, log the user in
            await _contentRepository
                .Users()
                .Authentication()
                .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
                {
                    UserId = authResult.User.UserId,
                    RememberUser = true
                });

            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return await GetLoggedInDefaultRedirectActionAsync();
        }

        public async Task<ActionResult> Logout()
        {
            await _contentRepository
                .Users()
                .Authentication()
                .SignOutAsync();

            return Redirect(_adminRouteLibrary.Auth.Login());
        }

        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (await IsLoggedInAsync()) return await GetLoggedInDefaultRedirectActionAsync();

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, new ForgotPasswordViewModel { Username = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code,
                    Username = command.Username
                });

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ForgotPassword));
            return View(viewPath, command);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword()
        {
            if (await IsLoggedInAsync()) return await GetLoggedInDefaultRedirectActionAsync();

            var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(Request.Query);

            var requestValidationResult = await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code,
                    Token = token
                })
                .ExecuteAsync();

            if (!ModelState.IsValid)
            {
                var invalidViewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword) + "RequestInvalid");
                return View(invalidViewPath, requestValidationResult);
            }

            var vm = new CompleteAccountRecoveryViewModel();
            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ResetPassword));

            return View(viewPath, vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(CompleteAccountRecoveryViewModel vm)
        {
            if (await IsLoggedInAsync()) return await GetLoggedInDefaultRedirectActionAsync();

            var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(Request.Query);
            
            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code,
                    Token = token,
                    NewPassword = vm.NewPassword
                });

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
            var redirectAction = await ValidateChangePasswordRouteAsync();
            if (redirectAction != null) return redirectAction;

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, new ChangePasswordViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string returnUrl, ChangePasswordViewModel vm)
        {
            var redirectAction = await ValidateChangePasswordRouteAsync();
            if (redirectAction != null) return redirectAction;

            await _contentRepository
                .WithModelState(this)
                .Users()
                .UpdatePasswordByCredentialsAsync(new UpdateUserPasswordByCredentialsCommand()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                });

            ViewBag.ReturnUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

            var viewPath = ViewPathFormatter.View(CONTROLLER_NAME, nameof(ChangePassword));
            return View(viewPath, vm);
        }

        private async Task<ActionResult> ValidateChangePasswordRouteAsync()
        {
            var user = await GetUserContextAsync();
            if (user.UserId.HasValue)
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return await GetLoggedInDefaultRedirectActionAsync();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _contentRepository
                    .Users()
                    .Authentication()
                    .SignOutAsync();
            }

            return null;
        }

        private async Task<bool> IsLoggedInAsync()
        {
            var userContext = await GetUserContextAsync();

            return userContext.IsSignedIn();
        }

        private Task<IUserContext> GetUserContextAsync()
        {
            return _contentRepository
                .Users()
                .Current()
                .Get()
                .AsUserContext()
                .ExecuteAsync();
        }

        private async Task<ActionResult> GetLoggedInDefaultRedirectActionAsync()
        {
            var modules = await _contentRepository.ExecuteQueryAsync(new GetPermittedAdminModulesQuery());
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