using Cofoundry.Domain;
using Cofoundry.Web;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("partners/auth")]
    public class PartnerAuthController : Controller
    {
        private readonly IAdvancedContentRepository _contentRepository;

        public PartnerAuthController(
            IAdvancedContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            // If you need to customize the model you can create your own 
            // that implements ILoginViewModel
            var viewModel = new SignInViewModel();

            return View(viewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInViewModel viewModel)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            // First authenticate the user without logging them in
            var authResult = await _contentRepository
                .WithModelState(this)
                .Users()
                .Authentication()
                .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
                {
                    UserAreaCode = CustomerUserArea.Code,
                    Username = viewModel.Username,
                    Password = viewModel.Password
                })
                .ExecuteAsync();

            if (!ModelState.IsValid)
            {
                // If the result isn't successful, the the ModelState will be populated
                // with an an error, but you could ignore ModelState handling and
                // instead add your own custom error views/messages by using authResult directly
                return View(viewModel);
            }

            var redirectUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

            if (authResult.User.RequirePasswordChange)
            {
                return RedirectToAction(nameof(ChangePassword), new { redirectUrl });
            }

            // If no action required, sign the user in
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

            return GetLoggedInDefaultRedirectAction();
        }

        [Route("change-password")]
        public async Task<ActionResult> ChangePassword()
        {
            var redirectAction = await ValidateChangePasswordRouteAsync();
            if (redirectAction != null) return redirectAction;
            
            // If you need to customize the model you can create your own 
            // that implements IChangePasswordViewModel
            var viewModel = new ChangePasswordViewModel();

            return View(viewModel);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var redirectAction = await ValidateChangePasswordRouteAsync();
            if (redirectAction != null) return redirectAction;

            await _contentRepository
                .WithModelState(this)
                .Users()
                .UpdatePasswordByCredentialsAsync(new UpdateUserPasswordByCredentialsCommand()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code,
                    Username = viewModel.Username,
                    NewPassword = viewModel.NewPassword,
                    OldPassword = viewModel.OldPassword
                });

            ViewBag.ReturnUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

            return View(viewModel);
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _contentRepository
                .Users()
                .Authentication()
                .SignOutAsync();

            return Redirect(UrlLibrary.PartnerSignIn());
        }

        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword()
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel command)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = PartnerUserArea.Code,
                    Username = command.Username
                });

            return View(command);
        }

        [Route("account-recovery")]
        public async Task<ActionResult> AccountRecovery([FromQuery] string t)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            var validationResult = await _contentRepository
                .Users()
                .AccountRecovery()
                .Validate(new ValidateUserAccountRecoveryByEmailQuery()
                {
                    UserAreaCode = PartnerUserArea.Code,
                    Token = t
                })
                .ExecuteAsync();

            if (!validationResult.IsSuccess)
            {
                return View(nameof(AccountRecovery) + "RequestInvalid", validationResult);
            }

            var vm = new CompleteAccountRecoveryViewModel();

            return View(vm);
        }

        [HttpPost("account-recovery")]
        public async Task<ActionResult> AccountRecovery(CompleteAccountRecoveryViewModel vm, [FromQuery] string t)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = PartnerUserArea.Code,
                    Token = t,
                    NewPassword = vm.ConfirmNewPassword
                });

            if (ModelState.IsValid)
            {
                return View(nameof(AccountRecovery) + "Complete");
            }

            return View(vm);
        }

        private async Task<ActionResult> GetRedirectIfSignedIn()
        {
            var isSignedIn = await _contentRepository
                .Users()
                .Current()
                .IsSignedIn()
                .ExecuteAsync();

            if (isSignedIn)
            {
                return GetLoggedInDefaultRedirectAction();
            }

            return null;
        }

        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerWelcome());
        }

        private async Task<ActionResult> ValidateChangePasswordRouteAsync()
        {
            var user = await _contentRepository
                .Users()
                .Current()
                .Get()
                .AsUserContext()
                .ExecuteAsync();

            if (user.UserId.HasValue)
            {
                if (!user.IsPasswordChangeRequired)
                {
                    return GetLoggedInDefaultRedirectAction();
                }

                // The user shouldn't be logged in, but if so, log them out
                await _contentRepository
                    .Users()
                    .Authentication()
                    .SignOutAsync();
            }

            return null;
        }
    }
}
