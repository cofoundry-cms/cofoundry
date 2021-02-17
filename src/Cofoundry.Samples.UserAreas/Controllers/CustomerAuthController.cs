using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("customers/auth")]
    public class CustomerAuthController : Controller
    {
        private readonly IAuthenticationControllerHelper<PartnerUserAreaDefinition> _authenticationControllerHelper;
        private readonly IUserContextService _userContextService;
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IMailService _mailService;

        public CustomerAuthController(
            IAuthenticationControllerHelper<PartnerUserAreaDefinition> authenticationControllerHelper,
            IAdvancedContentRepository contentRepository,
            IUserContextService userContextService,
            IExecutionContextFactory executionContextFactory,
            IMailService mailService
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
            _userContextService = userContextService;
            _contentRepository = contentRepository;
            _executionContextFactory = executionContextFactory;
            _mailService = mailService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        #region register

        [Route("register")]
        public async Task<IActionResult> Register()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var viewModel = new RegisterNewUserViewModel();

            return View(viewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterNewUserViewModel viewModel)
        {
            // TODO: yah: pull in content repository code and improve this,

            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn())
            {
                ModelState.AddModelError(string.Empty, "You cannot register because you are already logged in.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            var addUserCommand = new AddUserCommand()
            {
                Email = viewModel.Email,
                Password = viewModel.Password,
                RoleCode = CustomerRoleDefinition.Code,
                UserAreaCode = CustomerUserAreaDefinition.Code
                // DisplayName = 
            };

            // Because we're not logged in, we'll need to elevate permissions to 
            // add a new user account. Using "WithElevatedPermissions" make the
            // command is executed with the system user account.

            using (var scope = _contentRepository.Transactions().CreateScope())
            {
                await _contentRepository
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(addUserCommand);

                await scope.CompleteAsync();
            }

            // TODO: Verify Email
            var welcomeEmailTemplate = new NewUserWelcomeMailTemplate()
            {
                DisplayName = "TODO",
                EmailVerificationUrl = "TODO"
            };

            await _mailService.SendAsync(viewModel.Email, welcomeEmailTemplate);
            //}
            //await _loginService.LogAuthenticatedUserInAsync(addUserCommand.UserAreaCode, addUserCommand.OutputUserId, true);


            return View(viewModel);
        }

        #endregion

        #region login/logout

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
            // First authenticate the user without logging them in
            var authResult = await _authenticationControllerHelper.AuthenticateAsync(this, viewModel);

            if (!authResult.IsSuccess)
            {
                // If the result isn't successful, the helper will have already populated 
                // the ModelState with an error, but you could ignore ModelState and
                // add your own custom error views/messages instead.

                return View(viewModel);
            }

            // An example of using custom logic at login to verify a user has 
            // confirmed their email before logging them in.
            if (!authResult.User.IsEmailConfirmed)
            {
                return Redirect(nameof(EmailVerificationRequired));
            }
            
            // If no action required, log the user in
            await _authenticationControllerHelper.LogUserInAsync(this, authResult.User, true);

            // Support redirect urls from login
            var redirectUrl = _authenticationControllerHelper.GetAndValidateReturnUrl(this);
            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return GetLoggedInDefaultRedirectAction();
        }

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _authenticationControllerHelper.LogoutAsync();
            return Redirect(UrlLibrary.PartnerLogin());
        }

        #endregion

        #region forgot password / reset

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

        #endregion

        #region verify email

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            return View();
        }

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired(LoginViewModel viewModel)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(PartnerUserAreaDefinition.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            // TODO: Verify user and re-send email


            return View();
        }

        [Route("verify-email")]
        public async Task<ActionResult> VerifyEmail()
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

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail(CompletePasswordResetViewModel vm)
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

        #endregion

        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerDefault());
        }
    }
}
