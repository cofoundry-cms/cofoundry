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
        private readonly IAuthenticationControllerHelper<PartnerUserAreaDefinition> _authenticationControllerHelper;
        private readonly IUserContextService _userContextService;

        public PartnerAuthController(
            IAuthenticationControllerHelper<PartnerUserAreaDefinition> authenticationControllerHelper,
            IUserContextService userContextService
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
            _userContextService = userContextService;
        }

        [Route("")]
        [Route("/partners")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            // If you need to customize the model you can create your own 
            // that implements ILoginViewModel
            var viewModel = new LoginViewModel();

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var loginResult = await _authenticationControllerHelper.LogUserInAsync(this, viewModel);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost("forgot-password")]
        public async Task<ViewResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            // We need to pass in the url to our custom password reset action, which 
            // will then get included in the email notification
            var resetUrl = new Uri("/partners/auth/password-reset", UriKind.Relative);
            await _authenticationControllerHelper.SendPasswordResetNotificationAsync(this, command, resetUrl);

            return View(command);
        }

        [Route("password-reset")]
        public async Task<ActionResult> PasswordReset()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
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
            return Redirect(UrlLibrary.PartnerDefault());
        }
    }
}
