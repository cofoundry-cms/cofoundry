using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns information about a user if the specified credentials
    /// pass an authentication check.
    /// </summary>
    public class GetUserLoginInfoIfAuthenticatedQuery : IQuery<UserLoginInfoAuthenticationResult>
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> to match logins for. Note
        /// that usernames are unique per user area and therefore a given username
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
    }
}
