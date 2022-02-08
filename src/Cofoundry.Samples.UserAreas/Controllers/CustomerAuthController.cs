using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("customers/auth")]
    public class CustomerAuthController : Controller
    {
        private readonly IAuthenticationControllerHelper<CustomerUserArea> _authenticationControllerHelper;
        private readonly IUserContextService _userContextService;
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IMailService _mailService;
        private readonly IControllerResponseHelper _controllerResponseHelper;

        public CustomerAuthController(
            IAuthenticationControllerHelper<CustomerUserArea> authenticationControllerHelper,
            IAdvancedContentRepository contentRepository,
            IUserContextService userContextService,
            IExecutionContextFactory executionContextFactory,
            IMailService mailService,
            IControllerResponseHelper controllerResponseHelper
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
            _userContextService = userContextService;
            _contentRepository = contentRepository;
            _executionContextFactory = executionContextFactory;
            _mailService = mailService;
            _controllerResponseHelper = controllerResponseHelper;
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Login));
        }

        [Route("register")]
        public async Task<IActionResult> Register()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var viewModel = new RegisterNewUserViewModel();

            return View(viewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterNewUserViewModel viewModel)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn())
            {
                ModelState.AddModelError(string.Empty, "You cannot register because you are already logged in.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            await ExecuteInTransaction(this, async repository =>
            {
                var userId = await repository
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(new AddUserCommand()
                    {
                        Email = viewModel.Email,
                        Password = viewModel.Password,
                        RoleCode = CustomerRole.Code,
                        UserAreaCode = CustomerUserArea.Code,
                    });

                await repository
                    .Users()
                    .AccountVerification()
                    .EmailFlow()
                    .InitiateAsync(new InitiateUserAccountVerificationByEmailCommand()
                    {
                        UserId = userId
                    });
            });

            //using (var scope = _contentRepository.Transactions().CreateScope())
            //{
            //    // Because we're not logged in, we'll need to elevate permissions to 
            //    // add a new user account. Using "WithElevatedPermissions" make the
            //    // command is executed with the system user account.
            //    var userId = await _contentRepository
            //        .WithElevatedPermissions()
            //        .Users()
            //        .AddAsync(new AddUserCommand()
            //        {
            //            Email = viewModel.Email,
            //            Password = viewModel.Password,
            //            RoleCode = CustomerRole.Code,
            //            UserAreaCode = CustomerUserArea.Code,
            //        });

            //    await _contentRepository
            //        .Users()
            //        .AccountVerification()
            //        .EmailFlow()
            //        .InitiateAsync(new InitiateUserAccountVerificationByEmailCommand()
            //        {
            //            UserId = userId
            //        });

            //    await scope.CompleteAsync();
            //}

            return View(viewModel);
        }

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
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

            // An example of using custom logic at login to verify a user has 
            // confirmed their email before logging them in.
            if (!authResult.User.IsAccountVerified)
            {
                return Redirect(nameof(EmailVerificationRequired));
            }

            // If no action required, log the user in
            await _authenticationControllerHelper.SignInUserAsync(this, authResult.User, true);

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
            await _authenticationControllerHelper.SignOutAsync();
            return Redirect(UrlLibrary.PartnerSignIn());
        }

        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
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
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            await _authenticationControllerHelper.CompleteAccountRecoveryAsync(this, vm);

            if (ModelState.IsValid)
            {
                return View(nameof(AccountRecovery) + "Complete");
            }

            return View(vm);
        }

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            return View();
        }

        [Route("email-verification-required")]
        public async Task<ActionResult> EmailVerificationRequired(SignInViewModel viewModel)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            // TODO: Verify user and re-send email


            return View();
        }

        [Route("verify-email")]
        public async Task<ActionResult> VerifyEmail()
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            var requestValidationResult = await _authenticationControllerHelper.ParseAndValidateAccountRecoveryRequestAsync(this);

            if (!requestValidationResult.IsSuccess)
            {
                return View(nameof(VerifyEmail) + "RequestInvalid", requestValidationResult);
            }

            var vm = new CompleteAccountRecoveryViewModel(requestValidationResult);

            return View(vm);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail(CompleteAccountVerificationViewModel vm)
        {
            var user = await _userContextService.GetCurrentContextByUserAreaAsync(CustomerUserArea.Code);
            if (user.IsLoggedIn()) return GetLoggedInDefaultRedirectAction();

            await Execute(this, async repository =>
            {
                await repository
                    .Users()
                    .AccountVerification()
                    .EmailFlow()
                    .CompleteAsync(new CompleteUserAccountVerificationByEmailCommand()
                    {
                        UserAreaCode = CustomerUserArea.Code,
                        Token = vm.Token
                    });
            });

            if (ModelState.IsValid)
            {
                return View(nameof(VerifyEmail) + "Complete");
            }

            return View(vm);
        }

        private ActionResult GetLoggedInDefaultRedirectAction()
        {
            return Redirect(UrlLibrary.PartnerDefault());
        }

        #region controller helper ideas

        private async Task ExecuteInTransaction(Controller controller, Func<IAdvancedContentRepository, Task> actions)
        {
            using (var scope = _contentRepository.Transactions().CreateScope())
            {
                await Execute(controller, actions);
                await scope.CompleteAsync();
            }
        }

        private async Task Execute(Controller controller, Func<IAdvancedContentRepository, Task> actions)
        {
            if (controller.ModelState.IsValid)
            {
                try
                {
                    await actions(_contentRepository);
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToModelState(controller, ex);
                }
            }
        }

        private void AddValidationExceptionToModelState(Controller controller, ValidationException ex)
        {
            string propName = string.Empty;
            var prefix = controller.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }
            if (ex.ValidationResult != null && ex.ValidationResult.MemberNames.Count() == 1)
            {
                propName = prefix + ex.ValidationResult.MemberNames.First();
            }
            controller.ModelState.AddModelError(propName, ex.Message);
        }

        #endregion
    }
}