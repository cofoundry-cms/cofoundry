using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <inheritdoc/>
    public class AuthenticationControllerHelper<TUserArea> : IAuthenticationControllerHelper<TUserArea>
        where TUserArea : IUserAreaDefinition
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;
        private readonly IControllerResponseHelper _controllerResponseHelper;
        private readonly IAuthorizedTaskTokenUrlHelper _userAccountRecoveryUrlHelper;
        private readonly TUserArea _userAreaDefinition;

        public AuthenticationControllerHelper(
            IQueryExecutor queryExecutor,
            ILoginService loginService,
            IControllerResponseHelper controllerResponseHelper,
            IAuthorizedTaskTokenUrlHelper userAccountRecoveryUrlHelper,
            TUserArea userAreaDefinition
            )
        {
            _queryExecutor = queryExecutor;
            _loginService = loginService;
            _controllerResponseHelper = controllerResponseHelper;
            _userAccountRecoveryUrlHelper = userAccountRecoveryUrlHelper;
            _userAreaDefinition = userAreaDefinition;
        }

        public async Task<UserCredentialsValidationResult> AuthenticateAsync(Controller controller, ILoginViewModel viewModel)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (!controller.ModelState.IsValid)
            {
                return UserCredentialsValidationResult.CreateFailedResult();
            }

            var query = new ValidateUserCredentialsQuery()
            {
                UserAreaCode = _userAreaDefinition.UserAreaCode,
                Username = viewModel.Username,
                Password = viewModel.Password
            };

            var result = await _queryExecutor.ExecuteAsync(query);

            if (!result.IsSuccess)
            {
                controller.ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            return result;
        }

        public Task LogUserInAsync(
            Controller controller,
            UserLoginInfo user,
            bool rememberUser
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (user.RequirePasswordChange)
            {
                throw new PasswordChangeRequiredException();
            }

            if (_userAreaDefinition.UserAreaCode != user.UserAreaCode)
            {
                throw new ValidationException("This user account is not permitted to log in via this route.");
            }

            return _loginService.LogAuthenticatedUserInAsync(
                _userAreaDefinition.UserAreaCode,
                user.UserId,
                rememberUser
                );
        }

        /// <summary>
        /// Attempts to authenticate and log a user in using the data in the 
        /// specified view model, returning the result. ModelState is first 
        /// checked to be valid before checking the auth data against the database.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="viewModel">The view-model data posted to the action.</param>
        /// <returns>The result of the authentication check.</returns>
        //public async Task<UserLoginInfoAuthenticationResult> AuthenticateAndLogUserInAsync(Controller controller, ILoginViewModel viewModel)
        //{
        //    if (controller == null) throw new ArgumentNullException(nameof(controller));
        //    if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        //    if (!controller.ModelState.IsValid) return LoginResult.Failed;

        //    var command = new LogUserInWithCredentialsCommand()
        //    {
        //        UserAreaCode = _userAreaDefinition.UserAreaCode,
        //        Username = viewModel.Username,
        //        Password = viewModel.Password,
        //        RememberUser = viewModel.RememberMe
        //    };

        //    try
        //    {
        //        await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        //    }
        //    catch (PasswordChangeRequiredException ex)
        //    {
        //        // Add modelstate error as a precaution, because
        //        // result.RequiresPasswordChange may not be handled by the caller
        //        controller.ModelState.AddModelError(string.Empty, "Password change required.");

        //        return LoginResult.PasswordChangeRequired;
        //    }

        //    if (controller.ModelState.IsValid) return LoginResult.Success;

        //    return LoginResult.Failed;
        //}

        public string GetAndValidateReturnUrl(Controller controller)
        {
            var returnUrl = controller.Request.Query["ReturnUrl"].FirstOrDefault();

            if (!string.IsNullOrEmpty(returnUrl)
                && controller.Url.IsLocalUrl(returnUrl)
                && !RelativePathHelper.IsWellFormattedAndEqual(controller.Request.Path, returnUrl)
                )
            {
                return returnUrl;
            }

            return null;
        }

        public async Task ChangePasswordAsync(
            Controller controller,
            IChangePasswordViewModel vm
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            if (controller.ModelState.IsValid)
            {
                var command = new UpdateUserPasswordByCredentialsCommand()
                {
                    UserAreaCode = _userAreaDefinition.UserAreaCode,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                };

                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
        }

        public Task LogoutAsync()
        {
            return _loginService.SignOutAsync(_userAreaDefinition.UserAreaCode);
        }

        public Task SendAccountRecoveryNotificationAsync(
            Controller controller,
            IForgotPasswordViewModel vm
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;

            var command = new InitiateUserAccountRecoveryByEmailCommand()
            {
                Username = vm.Username,
                UserAreaCode = _userAreaDefinition.UserAreaCode
            };

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        public async Task<AccountRecoveryRequestValidationResult> ParseAndValidateAccountRecoveryRequestAsync(
            Controller controller
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            var query = new ValidateUserAccountRecoveryByEmailQuery()
            {
                UserAreaCode = _userAreaDefinition.UserAreaCode
            };

            if (!controller.ModelState.IsValid)
            {
                return await ValidateAndMapAsync(query);
            }

            // Parse the auth tokens from the request
            query.Token = _userAccountRecoveryUrlHelper.ParseTokenFromQuery(controller.Request.Query);

            var result = await ValidateAndMapAsync(query);

            if (result.Error != null)
            {
                controller.ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            return result;
        }

        private async Task<AccountRecoveryRequestValidationResult> ValidateAndMapAsync(ValidateUserAccountRecoveryByEmailQuery query)
        {
            var validationResult = await _queryExecutor.ExecuteAsync(query);

            var result = new AccountRecoveryRequestValidationResult();
            result.Token = query.Token;
            result.IsSuccess = validationResult.IsSuccess;
            result.Error = validationResult.Error;

            return result;
        }

        public Task CompleteAccountRecoveryAsync(
            Controller controller,
            ICompleteAccountRecoveryViewModel vm
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;

            var command = new CompleteUserAccountRecoveryByEmailCommand()
            {
                NewPassword = vm.NewPassword,
                Token = vm.Token,
                UserAreaCode = _userAreaDefinition.UserAreaCode
            };

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }
    }
}