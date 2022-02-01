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
        /// pertinent to login is returned, otherwise error information is returned detailing
        /// why the authentication failed.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<UserCredentialsValidationResult> ValidateCredentialsAsync(ValidateUserCredentialsQuery query);

        /// <summary>
        /// Completes an account recovery request initiated by <see cref="InitiateAsync"/>, 
        /// updating the users password if the request is verified.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task LogUserInWithCredentialsAsync(LogUserInWithCredentialsCommand command);
    }
}
