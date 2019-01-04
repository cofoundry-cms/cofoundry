using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Domain.MailTemplates;
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

        public async Task<AuthenticationResult> LogUserInAsync(Controller controller, ILoginViewModel vm, IUserAreaDefinition userAreaToLogInTo)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (userAreaToLogInTo == null) throw new ArgumentNullException(nameof(userAreaToLogInTo));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            var result = new AuthenticationResult();
            result.ReturnUrl = GetAndValidateReturnUrl(controller);

            if (!controller.ModelState.IsValid) return result;

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
                result.RequiresPasswordChange = true;
                // Add modelstate error as a precaution, because
                // result.RequiresPasswordChange may not be handled by the caller
                controller.ModelState.AddModelError(string.Empty, "Password change required.");
            }

            result.IsAuthenticated = controller.ModelState.IsValid;

            return result;
        }

        private static string GetAndValidateReturnUrl(Controller controller)
        {
            var returnUrl = controller.Request.Query["ReturnUrl"].FirstOrDefault();

            if (!string.IsNullOrEmpty(returnUrl) && controller.Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return null;
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

        public Task SendPasswordResetNotificationAsync<TNotificationViewModel>(Controller controller, IForgotPasswordViewModel vm, TNotificationViewModel notificationTemplate, IUserAreaDefinition userArea) 
            where TNotificationViewModel : IResetPasswordTemplate
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;

            var command = new ResetUserPasswordByUsernameCommand();
            command.Username = vm.Username;
            command.UserAreaCode = userArea.UserAreaCode;
            command.MailTemplate = notificationTemplate;

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        public async Task<PasswordResetRequestAuthenticationResult> IsPasswordRequestValidAsync(Controller controller, string requestId, string token, IUserAreaDefinition userAreaToLogInTo)
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

        public Task CompletePasswordResetAsync<TNotificationTemplate>(Controller controller, ICompletePasswordResetViewModel vm, TNotificationTemplate notificationTemplate, IUserAreaDefinition userAreaToLogInTo) where TNotificationTemplate : IPasswordChangedTemplate
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;
            
            var command = new CompleteUserPasswordResetCommand();

            Guid requestGuid;
            if (!Guid.TryParse(vm.UserPasswordResetRequestId, out requestGuid))
            {
                AddPasswordRequestInvalidError(controller);
                return Task.CompletedTask;
            }

            command.NewPassword = vm.NewPassword;
            command.Token = vm.Token;
            command.MailTemplate = notificationTemplate;
            command.UserPasswordResetRequestId = requestGuid;
            command.UserAreaCode = userAreaToLogInTo.UserAreaCode;

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        private static void AddPasswordRequestInvalidError(Controller controller)
        {
            controller.ModelState.AddModelError(string.Empty, "Invalid password request");
        }

        #endregion
    }
}