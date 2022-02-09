using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A helper class with shared functionality between controllers
    /// that manage user sign in.
    /// </summary>
    public interface IAuthenticationControllerHelper<TUserArea>
        where TUserArea : IUserAreaDefinition
    {

        /// <summary>
        /// Attempts to authenticate the sign in request, returning the result. This
        /// does not log the user in and can be used instead of SignInUserAsync when 
        /// you want more control over the sign in workflow. 
        /// 
        /// ModelState is first checked to be valid before checking the auth data against 
        /// the database. An auth errors are added to the ModelState.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="viewModel">The view-model data posted to the action.</param>
        /// <returns>The result of the authentication check, this should never be null.</returns>
        Task<UserCredentialsAuthenticationResult> AuthenticateAsync(Controller controller, ISignInViewModel viewModel);

        /// <summary>
        /// Signs in a user that has already been authenticated, typically
        /// by the AuthenticateAsync method.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="user">A <see cref="UserSignInInfo"/> object that has been returned from a sucessful authentication request.</param>
        /// <param name="rememberUser">
        /// <see langword="true"/> if the user should stay signed in perminantely; otherwise <see langword="false"/>.
        /// if the user should only stay signed in for the duration of
        /// the browser session.
        /// </param>
        Task SignInUserAsync(Controller controller, UserSignInInfo user, bool rememberUser);

        /// <summary>
        /// Retreives the ASP.NET MVC standard "ReturnUrl" query parameter and
        /// validates it to be a local url before returning it.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <returns>The return url if it is a valid local url; otherwise null.</returns>
        string GetAndValidateReturnUrl(Controller controller);

        /// <summary>
        /// Used to change a users password when it is required before sign in. Once
        /// completed the user should be redirected back to sign in to re-authenticate.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model containing the data entered by the user.</param>
        Task ChangePasswordAsync(
            Controller controller,
            IChangePasswordViewModel vm
            );

        /// <summary>
        /// Signs the user out of the user area.
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Checks the ModelState is valid and then initiates
        /// an account recovery (AKA "forgot password") request.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model data posted to the action.</param>
        Task SendAccountRecoveryNotificationAsync(
            Controller controller,
            IForgotPasswordViewModel vm
            );

        /// <summary>
        /// Parses the account recovery authentication parameters out of the request
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
        Task<AccountRecoveryRequestValidationResult> ParseAndValidateAccountRecoveryRequestAsync(
            Controller controller
            );

        /// <summary>
        /// Completes an account recovery request, validating the ModelState and
        /// view-model data before updating the database and sending
        /// a confirmation notification.
        /// </summary>
        /// <param name="controller">
        /// This method is intended to be called from an MVC controller and this
        /// should be the controller instance.
        /// </param>
        /// <param name="vm">The view-model data posted to the action.</param>
        Task CompleteAccountRecoveryAsync(
            Controller controller,
            ICompleteAccountRecoveryViewModel vm
            );
    }
}