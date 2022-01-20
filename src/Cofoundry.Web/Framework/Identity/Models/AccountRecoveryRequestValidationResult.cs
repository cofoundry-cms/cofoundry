using Cofoundry.Domain;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// Contains the result of an authentication test on an account
    /// recovery token.
    /// </summary>
    public class AccountRecoveryRequestValidationResult : ValidationQueryResult
    {
        /// <summary>
        /// The token used in the authentication attempt.
        /// </summary>
        public string Token { get; set; }
    }
}
