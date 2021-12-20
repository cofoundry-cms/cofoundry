using Cofoundry.Domain;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("partners/auth")]
    public class PartnerAuthController : Controller
    {
        private readonly IAuthenticationControllerHelper<PartnerUserArea> _authenticationControllerHelper;
        private readonly IUserContextService _userContextService;

        public PartnerAuthController(
            IAuthenticationControllerHelper<PartnerUserArea> authenticationControllerHelper,
            IUserContextService userContextService
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            // If you need to customize the model you can create your own 
            // that implements ILoginViewModel
            var viewModel = new LoginViewModel();

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            // First authenticate the user without logging them in
            var authResult = await _authenticationControllerHelper.AuthenticateAsync(this, viewModel);

            if (!authResult.IsSuccess)
            {
                // If the result isn't successful, the helper will have already populated 
                // the ModelState with an error, but you could ignore ModelState and
                // add your own custom error views/messages instead.

                return View(viewModel);
            }

            // Support redirect urls from login
            var redirectUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            if (authResult.User.RequirePasswordChange)
            {
                return RedirectToAction(nameof(ChangePassword), new { redirectUrl });
            }

            // If no action required, log the user in
            await _authenticationControllerHelper.LogUserInAsync(this, authResult.User, true);

            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return GetLoggedInDefaultRedirectAction();
        }

        [Route("change-password")]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn())
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return GetLoggedInDefaultRedirectAction();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync();
            }

            // If you need to customize the model you can create your own 
            // that implements IChangePasswordViewModel
            var viewModel = new ChangePasswordViewModel();

            return View(viewModel);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn())
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return GetLoggedInDefaultRedirectAction();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _authenticationControllerHelper.LogoutAsync();
            }
            await _authenticationControllerHelper.ChangePasswordAsync(this, viewModel);

            ViewBag.ReturnUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            return View(viewModel);
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authenticationControllerHelper.LogoutAsync();
            return Redirect(UrlLibrary.PartnerLogin());
        }

        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost("forgot-password")]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            // We need to pass in the url to our custom password reset action, which 
            // will then get included in the email notification
            await _authenticationControllerHelper.SendPasswordResetNotificationAsync(
                this,
                command,
                "/partners/auth/password-reset"
                );

            return View(command);
        }

        [Route("password-reset")]
        public async Task<ActionResult> PasswordReset()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var requestValidationResult = await _authenticationControllerHelper.ParseAndValidatePasswordResetRequestAsync(this);

            if (!requestValidationResult.IsValid)
            {
                return View(nameof(PasswordReset) + "RequestInvalid", requestValidationResult);
            }

            var vm = new CompletePasswordResetViewModel(requestValidationResult);

            return View(vm);
        }

        [HttpPost("password-reset")]
        public async Task<ActionResult> PasswordReset(CompletePasswordResetViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            await _authenticationControllerHelper.CompletePasswordResetAsync(this, vm);

            if (ModelState.IsValid)
            {
                return View(nameof(PasswordReset) + "Complete");
            }

            return View(vm);
        }


        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerWelcome());
        }
    }
}
