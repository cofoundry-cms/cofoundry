using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A helper class with shared functionality between controllers
    /// that manage user login.
    /// </summary>
    public class AuthenticationControllerHelper
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILoginService _loginService;
        private readonly IUserContextService _userContextService;
        private readonly IControllerResponseHelper _controllerResponseHelper;

        public AuthenticationControllerHelper(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            ILoginService loginService,
            IControllerResponseHelper controllerResponseHelper,
            IUserContextService userContextService
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _loginService = loginService;
            _controllerResponseHelper = controllerResponseHelper;
            _userContextService = userContextService;
        }

        #endregion

        #region Log in

        public async Task<LoginResult> LogUserInAsync(Controller controller, ILoginViewModel vm, IUserAreaDefinition userAreaToLogInTo)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (userAreaToLogInTo == null) throw new ArgumentNullException(nameof(userAreaToLogInTo));
            if (vm == null) throw new ArgumentNullException(nameof(vm));
            
            if (!controller.ModelState.IsValid) return LoginResult.Failed;

            var command = new LogUserInWithCredentialsCommand()
            {
                UserAreaCode = userAreaToLogInTo.UserAreaCode,
                Username = vm.Username,
                Password = vm.Password,
                RememberUser = vm.RememberMe
            };

            try
            {
                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
            catch (PasswordChangeRequiredException ex)
            {
                // Add modelstate error as a precaution, because
                // result.RequiresPasswordChange may not be handled by the caller
                controller.ModelState.AddModelError(string.Empty, "Password change required.");

                return LoginResult.PasswordChangeRequired;
            }

            if (controller.ModelState.IsValid) return LoginResult.Sucess;

            return LoginResult.Failed;
        }

        public string GetAndValidateReturnUrl(Controller controller)
        {
            var returnUrl = controller.Request.Query["ReturnUrl"].FirstOrDefault();

            if (!string.IsNullOrEmpty(returnUrl) && controller.Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return null;
        }


        #endregion

        #region ChangePasswordAsync

        /// <summary>
        /// Changes a users password, sending them an email notification if the operation 
        /// was successful.
        /// </summary>
        /// <param name="controller">Controller instance</param>
        /// <param name="vm">The IChangePasswordTemplate containing the data entered by the user.</param>
        /// <param name="userArea">
        /// The user area that the user belongs to. Usernames are only unique by user area 
        /// so all user commands need to be run against a specific user area.
        /// </param>
        /// <returns>The user id of the updated user if the action was successful; otheriwse null.</returns>
        public async Task ChangePasswordAsync(
            Controller controller,
            IChangePasswordViewModel vm,
            IUserAreaDefinition userArea
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (userArea == null) throw new ArgumentNullException(nameof(userArea));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            if (controller.ModelState.IsValid)
            {
                var command = new UpdateUnauthenticatedUserPasswordCommand()
                {
                    UserAreaCode = userArea.UserAreaCode,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                };

                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
        }

        #endregion

        #region log out

        public Task LogoutAsync(IUserAreaDefinition userAreaToLogInTo)
        {
            if (userAreaToLogInTo == null) throw new ArgumentNullException(nameof(userAreaToLogInTo));

            return _loginService.SignOutAsync(userAreaToLogInTo.UserAreaCode);
        }

        #endregion

        #region forgot password

        public Task SendPasswordResetNotificationAsync(
            Controller controller, 
            IForgotPasswordViewModel vm, 
            IUserAreaDefinition userArea
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;

            var command = new InitiatePasswordResetRequestCommand()
            {
                Username = vm.Username,
                UserAreaCode = userArea.UserAreaCode
            };

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        public async Task<PasswordResetRequestAuthenticationResult> IsPasswordRequestValidAsync(
            Controller controller, 
            string requestId, 
            string token, 
            IUserAreaDefinition userAreaToLogInTo
            )
        {
            var result = new PasswordResetRequestAuthenticationResult();
            result.ValidationErrorMessage = "Invalid password reset request";

            if (!controller.ModelState.IsValid) return result;

            if (string.IsNullOrWhiteSpace(requestId) || string.IsNullOrWhiteSpace(token))
            {
                AddPasswordRequestInvalidError(controller);
                return result;
            }

            Guid requestGuid;
            if (!Guid.TryParse(requestId, out requestGuid))
            {
                AddPasswordRequestInvalidError(controller);
                return result;
            }

            var query = new ValidatePasswordResetRequestQuery();
            query.UserPasswordResetRequestId = requestGuid;
            query.Token = Uri.UnescapeDataString(token);
            query.UserAreaCode = userAreaToLogInTo.UserAreaCode;

            result = await _queryExecutor.ExecuteAsync(query);

            return result;
        }

        public Task CompletePasswordResetAsync(
            Controller controller, 
            ICompletePasswordResetViewModel vm, 
            IUserAreaDefinition userAreaToLogInTo
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;
            
            Guid requestGuid;
            if (!Guid.TryParse(vm.UserPasswordResetRequestId, out requestGuid))
            {
                AddPasswordRequestInvalidError(controller);
                return Task.CompletedTask;
            }

            var command = new CompleteUserPasswordResetCommand()
            {
                NewPassword = vm.NewPassword,
                Token = vm.Token,
                UserPasswordResetRequestId = requestGuid,
                UserAreaCode = userAreaToLogInTo.UserAreaCode
            };

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        private static void AddPasswordRequestInvalidError(Controller controller)
        {
            controller.ModelState.AddModelError(string.Empty, "Invalid password request");
        }

        #endregion
    }
}