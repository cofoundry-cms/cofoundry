using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for self-service account recovery (AKA "forgot password") requests.
    /// </summary>
    public interface IAdvancedContentRepositoryUserAccountRecoveryRepository
    {
        /// <summary>
        /// Determines if an account recovery request is valid. The result is returned as a 
        /// <see cref="AuthorizedTaskTokenValidationResult"/> which describes any errors that have occurred.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> Validate(ValidateUserAccountRecoveryByEmailQuery query);

        /// <summary>
        /// <para>
        /// Initiates an account recovery (AKA "forgot password") request, sending a 
        /// notification to the user with a url to an account recovery form. This command
        /// is designed for self-service password reset so the password is not changed 
        /// until the form has been completed. 
        /// </para>
        /// <para>
        /// Requests are logged and validated to prevent too many account recovery
        /// attempts being initiated in a set period of time.
        /// </para>
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task InitiateAsync(InitiateUserAccountRecoveryViaEmailCommand command);

        /// <summary>
        /// Completes an account recovery request initiated by <see cref="InitiateAsync"/>, 
        /// updating the users password if the request is verified.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task CompleteAsync(CompleteUserAccountRecoveryViaEmailCommand command);
    }
}
