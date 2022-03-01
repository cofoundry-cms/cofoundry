using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for email-based account verification requests.
    /// </summary>
    public interface IAdvancedContentRepositoryUserAccountVerificationByEmailRepository
    {
        /// <summary>
        /// Determines if an email-based account verification request is valid. The result is returned as a 
        /// <see cref="AuthorizedTaskTokenValidationResult"/> which describes any errors that have occurred.
        /// </summary>
        /// <param name="token">The token used to verify the request.</param>
        IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> Validate(ValidateUserAccountVerificationByEmailQuery query);

        /// <summary>
        /// <para>
        /// Initiates an email-based account verification (AKA "confirm account") request, sending an 
        /// email notification to the user with a url to an account verification page. 
        /// </para>
        /// <para>
        /// This command utilises the "Authorized Task" framework to do most of the work, and if
        /// you want a more customized process then you may wish to use the authorized task framework 
        /// directly.
        /// </para>
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task InitiateAsync(InitiateUserAccountVerificationViaEmailCommand command);

        /// <summary>
        /// Initiates an email-based user account verification flow whereby validation is performed
        /// using a unique string token, which is typically inserted into a link in an email
        /// notification.
        /// </summary>
        /// <remarks>
        /// This command utilises the "Authorized Task" framework to do most of the work, and if
        /// you want a more customized process then you may wish to use the authorized task framework 
        /// directly.
        /// </remarks>
        /// <param name="command">Command parameters.</param>
        Task CompleteAsync(CompleteUserAccountVerificationViaEmailCommand command);
    }
}
