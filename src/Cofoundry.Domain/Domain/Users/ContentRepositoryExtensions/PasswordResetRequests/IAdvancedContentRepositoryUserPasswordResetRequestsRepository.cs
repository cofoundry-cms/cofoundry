using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryUserPasswordResetRequestsRepository
    {
        /// <summary>
        /// Determines if a password reset request is valid. The reslt is returned as a 
        /// <see cref="PasswordResetRequestAuthenticationResult"/> which uses 
        /// <see cref="PasswordResetRequestAuthenticationError"/> to describe specific types of error
        /// that can occur.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<PasswordResetRequestAuthenticationResult> Validate(ValidatePasswordResetRequestQuery query);

        /// <summary>
        /// <para>
        /// Initiates a password reset request, sending a notification
        /// to the user with a url to a password reset form. This command
        /// is designed for self-service password reset so the password
        /// is not changed until the form has been completed. 
        /// </para>
        /// <para>
        /// Requests are logged and validated to prevent too many reset
        /// attempts being initiated in a set period of time.
        /// </para>
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task InitiateAsync(InitiateUserPasswordResetRequestCommand command);

        /// <summary>
        /// Completes a password reset request initiated by <see cref="InitiateAsync"/>, 
        /// updating the users password if the request is verified.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task CompleteAsync(CompleteUserPasswordResetRequestCommand command);
    }
}
