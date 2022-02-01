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
    [Route("members")]
    public class MemberController : Controller
    {
        private readonly IAuthenticationControllerHelper<CustomerUserArea> _authenticationControllerHelper;
        private readonly IUserContextService _userContextService;
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IMailService _mailService;
        private readonly IControllerResponseHelper _controllerResponseHelper;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;

        public MemberController(
            IAuthenticationControllerHelper<CustomerUserArea> authenticationControllerHelper,
            IAdvancedContentRepository contentRepository,
            IUserContextService userContextService,
            IExecutionContextFactory executionContextFactory,
            IMailService mailService,
            IControllerResponseHelper controllerResponseHelper,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper
            )
        {
            _authenticationControllerHelper = authenticationControllerHelper;
            _userContextService = userContextService;
            _contentRepository = contentRepository;
            _executionContextFactory = executionContextFactory;
            _mailService = mailService;
            _controllerResponseHelper = controllerResponseHelper;
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> Invite(RegisterNewUserViewModel viewModel)
        {
            var user = await _contentRepository
                .Users()
                .Current()
                .Get()
                .AsUserContext()
                .ExecuteAsync();

            if (!user.IsLoggedIn())
            {
                ModelState.AddModelError(string.Empty, "You cannot invite a user.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            await ExecuteInTransaction(this, async repository =>
            {
                var token = await repository
                    .AuthorizedTasks()
                    .AddAsync(new AddAuthorizedTaskCommand()
                    {
                        AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                        UserId = user.UserId.Value,
                        TaskData = "cofoundry@example.com",
                        ExpireAfter = TimeSpan.FromDays(7),
                        RateLimitQuantity = 3,
                        RateLimitWindow = TimeSpan.FromHours(6)
                    });
            });

            return View(viewModel);
        }

        [Route("register")]
        public async Task<IActionResult> Register()
        {
            // Init an empty view model to avoid returning null
            var viewModel = new RegisterNewUserViewModel();

            // First validate that the current user isn't logged in
            var user = await _contentRepository
                .Users().Current().Get().AsUserContext()
                .ExecuteAsync();

            if (user.IsLoggedIn())
            {
                ModelState.AddModelError(string.Empty, "You cannot register because you are already logged in.");
                return View(viewModel);
            }

            // We use IAuthorizedTaskTokenUrlHelper to get the token from the query
            // however is optional and you can pass the token in any way you want
            var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(this.Request.Query);

            // Validating the token will return a result that describes any errors
            var result = await _contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    Token = token
                })
                .ExecuteAsync();

            // If not successful, add the error message to the ModelState.
            // Alternatively you can return the full error model to the view
            // or call result.ThrowIfNotSuccess() to throw an exception.
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

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

            return View(viewModel);
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