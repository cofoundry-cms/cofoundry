using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Signs a user into the application for a specified user area
    /// using username and password credentials. Checks for valid
    /// credentials and includes additional security checking such
    /// as preventing excessive authentication attempts. Validation errors
    /// are thrown as ValidationExceptions.
    /// </summary>
    public class SignInUserWithCredentialsCommand : ICommand
    {
        /// <summary>
        /// The username may be an email address or text string depending
        /// on the configuration of the user area.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The plain text representaion of the password to attempt
        /// to log in with.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The unique code of the user area to attempt to log into. Note
        /// that usernames are unique per user area and therefore a given username
        /// may have an account for more than one user area.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// True if the user should stay logged in perminantely; false
        /// if the user should only stay logged in for the duration of
        /// the browser session.
        /// </summary>
        public bool RememberUser { get; set; }
    }
}
