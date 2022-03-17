using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain;

/// <summary>
/// Options to control the behavior of any authentication cookies.
/// </summary>
public class CookieOptions
{
    /// <summary>
    /// <para>
    /// The interval at which the claims principal should be validated
    /// and refreshed, specified as a <see cref="TimeSpan"/> or in JSON
    /// configuration as a time format string e.g. "00:30:00" to represent
    /// 30 minutes. This is the equivalent of <see cref="Microsoft.AspNetCore.Identity.SecurityStampValidatorOptions.ValidationInterval"/>
    /// and is therefore primarily concerned with validating the security stamp
    /// and invalidating any out of date cookies after key user data has 
    /// changed (e.g. username or password), but it also refreshes the claims
    /// principal too.
    /// </para>
    /// <para>
    /// The default value is 30 minutes, which matches the default value in 
    /// <see cref="Microsoft.AspNetCore.Identity.SecurityStampValidatorOptions.ValidationInterval"/>.
    /// Reducing the interval decreases the window for stale cookie sessions or
    /// claims data, but increases the workload on the server in validating and
    /// reloading claims data from the database.
    /// </para>
    /// </summary>
    /// <remarks>
    /// A good description on the reasons for the <see cref="Microsoft.AspNetCore.Identity.SecurityStampValidatorOptions.ValidationInterval"/>
    /// default value can be found here: https://security.stackexchange.com/a/167833.
    /// </remarks>
    public TimeSpan ClaimsValidationInterval { get; set; } = new SecurityStampValidatorOptions().ValidationInterval;

    /// <summary>
    /// The text to use to namespace the auth cookie. The user area
    /// code will be appended to this to make the cookie name, e.g.
    /// "MyAppAuth_COF". By default the cookie namespace is created
    /// using characters from the entry assembly name of your application.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Copies the options to a new instance, which can be modified
    /// without altering the base settings. This is used for user area
    /// specific configuration.
    /// </summary>
    public CookieOptions Clone()
    {
        return new CookieOptions()
        {
            ClaimsValidationInterval = ClaimsValidationInterval,
            Namespace = Namespace
        };
    }
}
