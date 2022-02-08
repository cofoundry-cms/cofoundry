using Cofoundry.Domain;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var viewModel = new SignInViewModel();

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInViewModel viewModel)
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
            await _authenticationControllerHelper.SignInUserAsync(this, authResult.User, true);

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
                await _authenticationControllerHelper.SignOutAsync();
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
                await _authenticationControllerHelper.SignOutAsync();
            }
            await _authenticationControllerHelper.ChangePasswordAsync(this, viewModel);

            ViewBag.ReturnUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);

            return View(viewModel);
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authenticationControllerHelper.SignOutAsync();
            return Redirect(UrlLibrary.PartnerSignIn());
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
            await _authenticationControllerHelper.SendAccountRecoveryNotificationAsync(this, command);

            return View(command);
        }

        [Route("account-recovery")]
        public async Task<ActionResult> AccountRecovery()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var requestValidationResult = await _authenticationControllerHelper.ParseAndValidateAccountRecoveryRequestAsync(this);

            if (!requestValidationResult.IsSuccess)
            {
                return View(nameof(AccountRecovery) + "RequestInvalid", requestValidationResult);
            }

            var vm = new CompleteAccountRecoveryViewModel(requestValidationResult);

            return View(vm);
        }

        [HttpPost("account-recovery")]
        public async Task<ActionResult> AccountRecovery(CompleteAccountRecoveryViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            await _authenticationControllerHelper.CompleteAccountRecoveryAsync(this, vm);

            if (ModelState.IsValid)
            {
                return View(nameof(AccountRecovery) + "Complete");
            }

            return View(vm);
        }

        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerWelcome());
        }
    }
}
