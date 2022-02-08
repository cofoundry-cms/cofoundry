namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A view model for sign in credentials, used by the
    /// Cofoundry identity MVC controller helpers.
    /// </summary>
    public interface ISignInViewModel
    {
        /// <summary>
        /// The username may be an email address or text string depending
        /// on the configuration of the user area.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The plain text representaion of the password to attempt
        /// to log in with.
        /// </summary>
        string Password { get; set; }
    }
}