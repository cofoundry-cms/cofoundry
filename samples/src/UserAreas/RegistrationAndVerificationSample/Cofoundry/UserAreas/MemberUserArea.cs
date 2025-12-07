namespace RegistrationAndVerificationSample;

/// <summary>
/// Implementing IUserAreaDefinition defines your user area, registering it with
/// Cofoundry and making a user management area available in the admin site. This 
/// example defines a members user area that uses an email address and password to 
/// authenticate.
/// </summary>
public class MemberUserArea : IUserAreaDefinition
{
    /// <summary>
    /// By convention we add a constant for the user area code
    /// to make it easier to reference.
    /// </summary>
    public const string Code = "MEM";

    /// <summary>
    /// A 3 letter code that uniquely identifies this user area. The cofoundry 
    /// user are uses the code "COF" so you can pick anything else!
    /// </summary>
    public string UserAreaCode => Code;

    /// <summary>
    /// Display name of the area, used in the Cofoundry admin panel
    /// as the navigation link to manage your users. This should be singular
    /// because "Users" is appended to the link text.
    /// </summary>
    public string Name => "Member";

    /// <summary>
    /// Indicates if users in this area can sign in using a password. If this
    /// is false then sign in will typically be via SSO or some other method. If this is 
    /// true then the email field is mandatory, because it is required for account recovery.
    /// </summary>
    public bool AllowPasswordSignIn => true;

    /// <summary>
    /// Indicates whether the user should sign in using thier email address
    /// as the username. If this is false then a separate username can be 
    /// provided to sign in with.
    /// </summary>
    public bool UseEmailAsUsername => false;

    /// <summary>
    /// The path to redirect to if a user is not signed in, which is typically a
    /// page where the user can sign in. If set to null then a 403 (Forbidden) error 
    /// page will be returned instead.
    /// </summary>
    public string SignInPath => "/members/auth";

    /// <summary>
    /// Cofoundry creates an auth scheme for each user area, but only one can be the 
    /// default. For this sample we're only dealing with a single customer user area
    /// and so it should be set to true.
    /// </summary>
    public bool IsDefaultAuthScheme => true;

    /// <summary>
    /// Implement this method to change any additional options from
    /// their defaults.
    /// </summary>
    public void ConfigureOptions(UserAreaOptions options)
    {
        // To support account verification by email, we need to provide
        // a base url for the verification link that gets inserted into 
        // the email. The verification token is appended to this url.
        options.AccountVerification.VerificationUrlBase = "/members/registration/verify";

        // Setting this to true is optional, but it will mean that
        // if we try to sign in an unverified user then an exception
        // will be thrown.
        options.AccountVerification.RequireVerification = true;

        // Since our usernames are not email-based, we can use this field as 
        // the display name and avoid asking for it separately
        options.Username.UseAsDisplayName = true;
    }
}
