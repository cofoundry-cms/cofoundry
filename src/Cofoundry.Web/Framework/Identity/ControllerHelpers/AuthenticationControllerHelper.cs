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

        public AuthenticationResult LogUserIn(Controller controller, ILoginViewModel vm, IUserArea userAreaToLogInTo)
        {
            Condition.Requires(controller).IsNotNull();
            Condition.Requires(userAreaToLogInTo).IsNotNull();
            Condition.Requires(vm).IsNotNull();

            var result = new AuthenticationResult();
            result.ReturnUrl = controller.Request["ReturnUrl"];

            // If user logged into another area, log them out first
            var currentContext = _userContextService.GetCurrentContext() as IUserContext;
            var isLoggedIntoDifferentUserArea = currentContext.UserArea != null && currentContext.UserArea.UserAreaCode != userAreaToLogInTo.UserAreaCode;

            if (currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea)
            {
                result.IsAuthenticated = true;
                return result;
            }

            CheckLoginMaxLoginAttemptsNotExceeded(controller, userAreaToLogInTo.UserAreaCode, vm.EmailAddress);

            if (controller.ModelState.IsValid)
            {
                var user = Authenticate(vm.EmailAddress, vm.Password);

                if (user == null)
                {
                    _loginService.LogFailedLoginAttempt(userAreaToLogInTo.UserAreaCode, vm.EmailAddress);

                    controller.ModelState.AddModelError("EmailAddress", "The give username/password combination was invalid");
                }
                else if (ValidateLoginArea(controller, userAreaToLogInTo, user.UserAreaCode))
                {
                    result.IsAuthenticated = true;
                    _loginService.LogAuthenticatedUserIn(user.UserId, vm.RememberMe);

                    result.RequiresPasswordChange = user.RequirePasswordChange;
                }
            }

            return result;
        }

        private void CheckLoginMaxLoginAttemptsNotExceeded(Controller controller, string userAreaCode, string username)
        {
            if (!controller.ModelState.IsValid || string.IsNullOrWhiteSpace(username)) return;

            var query = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = userAreaCode,
                Username = username
            };

            if (_queryExecutor.Execute(query))
            {
                controller.ModelState.AddModelError("Password", "Too many failed login attempts have been detected, please try again later.");
            }
        }

        private bool ValidateLoginArea(Controller controller, IUserArea userAreaToLogInto, string userArea)
        {
            if (userAreaToLogInto.UserAreaCode != userArea)
            {
                controller.ModelState.AddModelError("EmailAddress", "This user account is not permitted to log in via this route.");
                return false;
            }

            return true;
        }

        private UserLoginInfo Authenticate(string email, string password)
        {
            var authCommand = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                Username = email,
                Password = password
            };
            var user = _queryExecutor.Execute(authCommand);

            return user;
        }

        #endregion

        #region log out

        public void Logout()
        {
            _loginService.SignOut();
        }

        #endregion

        #region forgot password

        public void SendPasswordResetNotification<TNotificationViewModel>(Controller controller, IForgotPasswordViewModel vm, TNotificationViewModel notificationTemplate, IUserArea userArea) 
            where TNotificationViewModel : IResetPasswordTemplate
        {
            if (!controller.ModelState.IsValid) return;

            var command = new ResetUserPasswordByUsernameCommand();
            command.Username = vm.Username;
            command.UserAreaCode = userArea.UserAreaCode;
            command.MailTemplate = notificationTemplate;

            _controllerResponseHelper.ExecuteIfValid(controller, command);
        }

        public PasswordResetRequestAuthenticationResult IsPasswordRequestValid(Controller controller, string requestId, string token, IUserArea userAreaToLogInTo)
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

        public void CompletePasswordReset<TNotificationTemplate>(Controller controller, ICompletePasswordResetViewModel vm, TNotificationTemplate notificationTemplate, IUserArea userAreaToLogInTo) where TNotificationTemplate : IPasswordChangedTemplate
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