using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Validates user credentials. If the authentication was successful then user information 
    /// pertinent to sign in is returned, otherwise error information is returned detailing
    /// why the authentication failed.
    /// </summary>
    public class AuthenticateUserCredentialsQuery : IQuery<UserCredentialsAuthenticationResult>
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area the user is expected 
        /// to belong to. Note that usernames are unique per user area and therefore a given username
        /// may have an account for more than one user area.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The username to use to locate the user. The value will be "uniquified"
        /// before making the comparison.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to authenticate the user account with.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional name of the property to return in any validation errors.
        /// </summary>
        public string PropertyToValidate { get; set; }
    }
}
