using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Domain.MailTemplates;

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

        public AuthenticationResult LogUserIn(Controller controller, ILoginViewModel vm, IUserAreaDefinition userAreaToLogInTo)
        {
            Condition.Requires(controller).IsNotNull();
            Condition.Requires(userAreaToLogInTo).IsNotNull();
            Condition.Requires(vm).IsNotNull();

            var result = new AuthenticationResult();
            result.ReturnUrl = controller.Request["ReturnUrl"];
            if (!controller.ModelState.IsValid) return result;

            var command = new LogUserInWithCredentialsCommand()
            {
                UserAreaCode = userAreaToLogInTo.UserAreaCode,
                Username = vm.EmailAddress,
                Password = vm.Password,
                RememberUser = vm.RememberMe
            };

            _controllerResponseHelper.ExecuteIfValid(controller, command);

            if (controller.ModelState.IsValid)
            {
                result.IsAuthenticated = true;
                var currentContext = _userContextService.GetCurrentContext();
                result.RequiresPasswordChange = currentContext.IsPasswordChangeRequired;
            }

            return result;
        }


        #endregion

        #region log out

        public void Logout()
        {
            _loginService.SignOut();
        }

        #endregion

        #region forgot password

        public void SendPasswordResetNotification<TNotificationViewModel>(Controller controller, IForgotPasswordViewModel vm, TNotificationViewModel notificationTemplate, IUserAreaDefinition userArea) 
            where TNotificationViewModel : IResetPasswordTemplate
        {
            if (!controller.ModelState.IsValid) return;

            var command = new ResetUserPasswordByUsernameCommand();
            command.Username = vm.Username;
            command.UserAreaCode = userArea.UserAreaCode;
            command.MailTemplate = notificationTemplate;

            _controllerResponseHelper.ExecuteIfValid(controller, command);
        }

        public PasswordResetRequestAuthenticationResult IsPasswordRequestValid(Controller controller, string requestId, string token, IUserAreaDefinition userAreaToLogInTo)
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

            result = _queryExecutor.Execute(query);

            return result;
        }

        public void CompletePasswordReset<TNotificationTemplate>(Controller controller, ICompletePasswordResetViewModel vm, TNotificationTemplate notificationTemplate, IUserAreaDefinition userAreaToLogInTo) where TNotificationTemplate : IPasswordChangedTemplate
        {
            if (!controller.ModelState.IsValid) return;
            
            var command = new CompleteUserPasswordResetCommand();

            Guid requestGuid;
            if (!Guid.TryParse(vm.UserPasswordResetRequestId, out requestGuid))
            {
                AddPasswordRequestInvalidError(controller);
                return;
            }

            command.NewPassword = vm.NewPassword;
            command.Token = vm.Token;
            command.MailTemplate = notificationTemplate;
            command.UserPasswordResetRequestId = requestGuid;
            command.UserAreaCode = userAreaToLogInTo.UserAreaCode;

            _controllerResponseHelper.ExecuteIfValid(controller, command);
        }

        private static void AddPasswordRequestInvalidError(Controller controller)
        {
            controller.ModelState.AddModelError(string.Empty, "Invalid password request");
        }

        #endregion
    }
}