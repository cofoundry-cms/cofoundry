using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("customers/auth")]
    public class CustomerAuthController : Controller
    {
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IMailService _mailService;

        public CustomerAuthController(
            IAdvancedContentRepository contentRepository,
            IMailService mailService
            )
        {
            _contentRepository = contentRepository;
            _mailService = mailService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [Route("register")]
        public async Task<IActionResult> Register()
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            var viewModel = new RegisterNewUserViewModel();

            return View(viewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterNewUserViewModel viewModel)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            using (var scope = _contentRepository.Transactions().CreateScope())
            {
                // Because we're not logged in, we'll need to elevate permissions to 
                // add a new user account. Using "WithElevatedPermissions" make the
                // command is executed with the system user account.
                var userId = await _contentRepository
                    .WithModelState(this)
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(new AddUserCommand()
                    {
                        UserAreaCode = CustomerUserArea.Code,
                        RoleCode = CustomerRole.Code,
                        Email = viewModel.Email,
                        Password = viewModel.Password,
                    });

                // For customers, we need to validate their account before we let them sign in
                await _contentRepository
                    .Users()
                    .AccountVerification()
                    .EmailFlow()
                    .InitiateAsync(new InitiateUserAccountVerificationByEmailCommand()
                    {
                        UserId = userId
                    });

                await scope.CompleteIfValidAsync(ModelState);
            }

            return View(viewModel);
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

            // An example of using custom logic at login to verify a user has 
            // confirmed their email before logging them in.
            if (!authResult.User.IsAccountVerified)
            {
                return Redirect(nameof(EmailVerificationRequired));
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

            // Support redirect urls from login
            var redirectUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);
            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return GetLoggedInDefaultRedirectAction();
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _contentRepository
                .Users()
                .Authentication()
                .SignOutAsync();

            return Redirect(UrlLibrary.CustomerSignOut());
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
                    UserAreaCode = CustomerUserArea.Code,
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
                    UserAreaCode = CustomerUserArea.Code,
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
                    UserAreaCode = CustomerUserArea.Code,
                    Token = t,
                    NewPassword = vm.ConfirmNewPassword
                });

            if (ModelState.IsValid)
            {
                return View(nameof(AccountRecovery) + "Complete");
            }

            return View(vm);
        }

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired()
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            return View();
        }

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired(SignInViewModel viewModel)
        {
            var redirect = await GetRedirectIfSignedIn();
            if (redirect != null) return redirect;

            // TODO: Verify user and re-send email


            return View();
        }

        [Route("verify-email")]
        public async Task<ActionResult> VerifyEmail(string t)
        {
            var validationResult = await _contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .Validate(new ValidateUserAccountVerificationByEmailQuery()
                {
                    UserAreaCode = CustomerUserArea.Code,
                    Token = t
                })
                .ExecuteAsync();

            if (!validationResult.IsSuccess)
            {
                return View(nameof(VerifyEmail) + "RequestInvalid", validationResult);
            }

            return View();
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmailPost([FromQuery] string t)
        {
            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountVerification()
                .EmailFlow()
                .CompleteAsync(new CompleteUserAccountVerificationByEmailCommand()
                {
                    UserAreaCode = CustomerUserArea.Code,
                    Token = t
                });

            if (ModelState.IsValid)
            {
                return View(nameof(VerifyEmail) + "Complete");
            }

            return View();
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
            return Redirect(UrlLibrary.PartnerDefault());
        }
    }
}