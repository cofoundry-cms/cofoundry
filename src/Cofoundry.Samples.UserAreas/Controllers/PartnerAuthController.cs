using Cofoundry.Domain;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("partner/auth")]
    public class PartnerAuthController : Controller
    {
        private static IUserAreaDefinition USER_AREA = new PartnerUserAreaDefinition();

        private readonly AuthenticationControllerHelper _authenticationControllerHelper;
        private readonly AccountManagementControllerHelper _accountManagementControllerHelper;
        private readonly IUserContextService _userContextService;

        public PartnerAuthController(
            AuthenticationControllerHelper authenticationControllerHelper,
            AccountManagementControllerHelper accountManagementControllerHelper,
            IUserContextService userContextService
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
            _accountManagementControllerHelper = accountManagementControllerHelper;
            _userContextService = userContextService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            // If you need to customize the model you can create your own 
            // that implements ILoginViewModel
            var viewModel = new LoginViewModel();

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var loginResult = await _authenticationControllerHelper.LogUserInAsync(this, viewModel, USER_AREA);
            var redirectUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            if (loginResult == LoginResult.PasswordChangeRequired)
            {
                return RedirectToAction(nameof(ChangePassword), new { redirectUrl });
            }
            else if (loginResult == LoginResult.Success && redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }
            else if (loginResult == LoginResult.Success)
            {
                return GetLoggedInDefaultRedirectAction();
            }

            return View(viewModel);
        }

        [Route("change-password")]
        public async Task<IActionResult> ChangePassword()
        {
            // USER_AREA.LoginPath
            // USER_AREA.ChangePasswordPath
            // USER_AREA.PostLoginRedirect
            // USER_AREA.ResetPasswordPath
            // USER_AREA.EmailConfirmationPath

            //var canLoginResult = _authenticationControllerHelper.ValidateCanLogin();
            //// CanLoginResult.Yes
            //// CanLoginResult.AlreadyLoggedIn
            //// CanLoginResult.RequiresPasswordChange
            //var checkResult = _authenticationControllerHelper.CanChangePassword();
            //// CanChangePasswordResult.Yes
            //// CanChangePasswordResult.DoesNotRequirePasswordChange
            //if (checkResult == CanChangePasswordResult.PostLoginRedirect)
            //{

            //}

            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn())
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return GetLoggedInDefaultRedirectAction();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync(USER_AREA);
            }

            // If you need to customize the model yu can create your own 
            // that implements IChangePasswordViewModel
            var viewModel = new ChangePasswordViewModel();

            return View(viewModel);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn())
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return GetLoggedInDefaultRedirectAction();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync(USER_AREA);
            }
            // TODO: this has been moved from the account helper, which should be a diff method
            await _authenticationControllerHelper.ChangePasswordAsync(this, viewModel, USER_AREA);

            // forgot password
            // confirm email
            // two factor auth
            /// TODO: 
            /// - Password reset - think about naming, add views, fix etc.
            /// - PAssword reset isn't necessarily part of the leanest default flow
            /// so maybe it shouldn't have a default template. Admin does require it,
            /// but then there's stuff like confirm email which is not default workflow
            /// but should be easy to add in (like forgot password). 
            /// Maybe less is more here. Template-wise you'd have to create the builder
            /// and the separate templates, but otherwise where do you sotp!
            /// - is this the best/leanest we can do here?
            /// - What about the above pre-command code - tidier?
            ///  - Need logout and password reset
            ///  - PRevent returnUrl redirect loop
            ///  - Can we avoid use of separate _accountManagementControllerHelper?
            ViewBag.ReturnUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            return View(viewModel);
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authenticationControllerHelper.LogoutAsync(USER_AREA);
            return Redirect(UrlLibrary.PartnerLogin());
        }

        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            return View(new ForgotPasswordViewModel { Username = email });
        }

        [HttpPost("forgot-password")]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            await _authenticationControllerHelper.SendPasswordResetNotificationAsync(this, command, USER_AREA);

            return View(command);
        }

        [Route("reset-password")]
        public async Task<ActionResult> ResetPassword(string i, string t)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var request = await _authenticationControllerHelper.IsPasswordRequestValidAsync(this, i, t, USER_AREA);

            if (!request.IsValid)
            {
                return View(nameof(ResetPassword) + "RequestInvalid", request);
            }

            var vm = new CompletePasswordResetViewModel();
            vm.UserPasswordResetRequestId = i;
            vm.Token = t;

            return View(vm);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(CompletePasswordResetViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(USER_AREA.UserAreaCode);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            await _authenticationControllerHelper.CompletePasswordResetAsync(this, vm, USER_AREA);

            if (ModelState.IsValid)
            {
                return View(nameof(ResetPassword) + "Complete");
            }

            return View(vm);
        }


        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerDefault());
        }
    }
}
