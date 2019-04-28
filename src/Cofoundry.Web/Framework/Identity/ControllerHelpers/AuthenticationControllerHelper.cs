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
    public class AuthenticationControllerHelper<TUserArea>
        : IAuthenticationControllerHelper<TUserArea>
        where TUserArea : IUserAreaDefinition
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILoginService _loginService;
        private readonly IUserContextService _userContextService;
        private readonly IControllerResponseHelper _controllerResponseHelper;
        private readonly IPasswordResetUrlHelper _passwordResetUrlHelper;
        private readonly TUserArea _userAreaDefinition;

        public AuthenticationControllerHelper(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            ILoginService loginService,
            IControllerResponseHelper controllerResponseHelper,
            IUserContextService userContextService,
            IPasswordResetUrlHelper passwordResetUrlHelper,
            TUserArea userAreaDefinition
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _loginService = loginService;
            _controllerResponseHelper = controllerResponseHelper;
            _userContextService = userContextService;
            _passwordResetUrlHelper = passwordResetUrlHelper;
            _userAreaDefinition = userAreaDefinition;
        }

        #endregion

        #region Log in

        /// <summary>
        /// Attempts to log a user in using the data in the specified view 
        /// model, returning the result. ModelState is first checked to be 
        /// valid before checking the auth data against the database.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model data posted to the action.</param>
        /// <returns></returns>
        public async Task<LoginResult> LogUserInAsync(Controller controller, ILoginViewModel vm)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (vm == null) throw new ArgumentNullException(nameof(vm));
            
            if (!controller.ModelState.IsValid) return LoginResult.Failed;

            var command = new LogUserInWithCredentialsCommand()
            {
                UserAreaCode = _userAreaDefinition.UserAreaCode,
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

            if (controller.ModelState.IsValid) return LoginResult.Success;

            return LoginResult.Failed;
        }

        /// <summary>
        /// Retreives the ASP.NET MVC standard "ReturnUrl" query parameter and
        /// validates it to be a local url before returning it.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <returns>The return url if it is a valid local url; otherwise null.</returns>
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
        /// Used to change a users password when it is required before login. Once
        /// completed the user should be redirected back to login to re-authenticate.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model containing the data entered by the user.</param>
        public async Task ChangePasswordAsync(
            Controller controller,
            IChangePasswordViewModel vm
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            if (controller.ModelState.IsValid)
            {
                var command = new UpdateUnauthenticatedUserPasswordCommand()
                {
                    UserAreaCode = _userAreaDefinition.UserAreaCode,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                };

                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
        }

        #endregion

        #region log out

        /// <summary>
        /// Signs the user out of the user area.
        /// </summary>
        public Task LogoutAsync()
        {
            return _loginService.SignOutAsync(_userAreaDefinition.UserAreaCode);
        }

        #endregion

        #region forgot password

        /// <summary>
        /// Checks the ModelState is valid and then initiates
        /// a password reset request.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model data posted to the action.</param>
        /// <param name="resetUrlBase">
        /// The relative base path used to construct the reset url 
        /// e.g. new Uri("/auth/forgot-password").
        /// </param>
        public Task SendPasswordResetNotificationAsync(
            Controller controller, 
            IForgotPasswordViewModel vm,
            Uri resetUrlBase
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;

            var command = new InitiatePasswordResetRequestCommand()
            {
                Username = vm.Username,
                UserAreaCode = _userAreaDefinition.UserAreaCode,
                ResetUrlBase = resetUrlBase
            };

            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        /// <summary>
        /// Parses the password reset authentication parameters out of the request
        /// url and validates them against the database before returning the result.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <returns>
        /// An object containing the validation result, details of any errors
        /// and the parsed authentication data.
        /// </returns>
        public async Task<PasswordResetRequestValidationResult> ParseAndValidatePasswordResetRequestAsync(
            Controller controller
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            
            var result = new PasswordResetRequestValidationResult();
            result.ValidationErrorMessage = "Invalid password reset request";

            if (!controller.ModelState.IsValid) return result;

            // Parse the auth tokens from the request
            var urlParameters = _passwordResetUrlHelper.ParseFromQuery(controller.Request.Query);
            result.UserPasswordResetRequestId = urlParameters.UserPasswordResetRequestId;
            result.Token = urlParameters.Token;

            // Check for missing parameters
            if (urlParameters.UserPasswordResetRequestId == Guid.Empty || string.IsNullOrWhiteSpace(urlParameters.Token))
            {
                AddPasswordRequestInvalidError(controller);
                return result;
            }

            // Validate the request against the db
            var query = new ValidatePasswordResetRequestQuery();
            query.UserPasswordResetRequestId = urlParameters.UserPasswordResetRequestId;
            query.Token = urlParameters.Token;
            query.UserAreaCode = _userAreaDefinition.UserAreaCode;

            var validationResult = await _queryExecutor.ExecuteAsync(query);

            result.IsValid = validationResult.IsValid;
            result.ValidationErrorMessage = validationResult.ValidationErrorMessage;

            return result;
        }

        /// <summary>
        /// Completes a password reset, validating the ModelState and
        /// view-model data before updating the database and sending
        /// a confirmation notification.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model data posted to the action.</param>
        public Task CompletePasswordResetAsync(
            Controller controller, 
            ICompletePasswordResetViewModel vm
            )
        {
            if (!controller.ModelState.IsValid) return Task.CompletedTask;
            
            if (vm.UserPasswordResetRequestId == Guid.Empty)
            {
                AddPasswordRequestInvalidError(controller);
                return Task.CompletedTask;
            }

            var command = new CompleteUserPasswordResetCommand()
            {
                NewPassword = vm.NewPassword,
                Token = vm.Token,
                UserPasswordResetRequestId = vm.UserPasswordResetRequestId,
                UserAreaCode = _userAreaDefinition.UserAreaCode
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