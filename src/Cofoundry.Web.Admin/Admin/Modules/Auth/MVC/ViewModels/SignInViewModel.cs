namespace Cofoundry.Web.Admin.Internal;

/// <summary>
/// A view model for user account sign in with user accounts that 
/// use email addresses as usernames.
/// </summary>
public class SignInViewModel
{
    /// <summary>
    /// A Cofoundry username can be an email address or a text string 
    /// depending on the configuration of the user area. This model
    /// is designed to support only email address for usernames.
    /// </summary>
    [DataType(DataType.EmailAddress)]
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    [Display(Name = "Email")]
    public string Username { get; set; }

    /// <summary>
    /// The plain text representaion of the password to attempt
    /// to log in with.
    /// </summary>
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please provide your password")]
    [Display(Name = "Password")]
    public string Password { get; set; }

    /// <summary>
    /// True if the user should stay logged in perminantely; false
    /// if the user should only stay logged in for the duration of
    /// the browser session.
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
