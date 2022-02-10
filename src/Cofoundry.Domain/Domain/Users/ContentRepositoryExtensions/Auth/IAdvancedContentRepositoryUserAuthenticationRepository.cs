using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for user authentication.
    /// </summary>
    public interface IAdvancedContentRepositoryUserAuthenticationRepository
    {
        /// <summary>
        /// Validates user credentials. If the authentication was successful then user information 
        /// pertinent to sign in is returned, otherwise error information is returned detailing
        /// why the authentication failed.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<UserCredentialsAuthenticationResult> AuthenticateCredentials(AuthenticateUserCredentialsQuery query);

        /// <summary>
        /// Signs a user into the application for a specified user area
        /// using username and password credentials to authenticate. Additional
        /// security checks are also made such as preventing excessive authentication 
        /// attempts. Validation errors are thrown as <see cref="ValidationErrorException"/>. 
        /// The ambient user area (i.e. "current" user context) is switched to the specified area 
        /// for the remainder of the DI scope (i.e. request for web apps).
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task SignInWithCredentialsAsync(SignInUserWithCredentialsCommand command);

        /// <summary>
        /// Signs in a user that has already passed an authentication check. The user 
        /// should have already passed authentication prior to calling this method. The 
        /// ambient user area (i.e. "current" user context) is switched to the specified area 
        /// for the remainder of the DI scope (i.e. request for web apps).
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task SignInAuthenticatedUserAsync(SignInAuthenticatedUserCommand command);

        /// <summary>
        /// Signs out the user currently logged into the "ambient" scheme, ending the session.
        /// The ambient scheme usually represents the default user area, unless it has been switched
        /// during the request. A <see cref="UserSignedOutMessage"/> is published once the user is 
        /// signed out; if the user is not signed in, no action is taken.
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Signs the user out of all user areas and ends the session. A
        /// <see cref="UserSignedOutMessage"/> is published for each user
        /// area that is logged out.
        /// </summary>
        Task SignOutAllUserAreasAsync();
    }
}