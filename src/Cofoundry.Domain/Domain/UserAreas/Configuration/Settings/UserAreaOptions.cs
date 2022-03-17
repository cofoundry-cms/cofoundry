namespace Cofoundry.Domain;

/// <summary>
/// Configuration options for a user area, which are based on 
/// <see cref="UsersSettings"/> and can be customized via
/// <see cref="IUserAreaDefinition.ConfigureOptions(UserAreaOptions)"/>.
/// </summary>
public class UserAreaOptions
{
    /// <summary>
    /// Options used in the default password validation logic.
    /// </summary>
    public PasswordOptions Password { get; set; }

    /// <summary>
    /// Options to control the formatting and validation of usernames.
    /// Note that username character validation is ignored when 
    /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
    /// <see langword="true"/>, because the format is already validated against
    /// the configured <see cref="EmailAddressOptions"/>.
    /// </summary>
    public UsernameOptions Username { get; set; }

    /// <summary>
    /// Options to control the formatting and validation of user email 
    /// addresses.
    /// </summary>
    public EmailAddressOptions EmailAddress { get; set; }

    /// <summary>
    /// Options to control the behavior of any authentication cookies.
    /// </summary>
    public CookieOptions Cookies { get; set; }

    /// <summary>
    /// Options to control the behavior of the authentication process and related
    /// security features
    /// </summary>
    public AuthenticationOptions Authentication { get; set; }

    /// <summary>
    /// Options to control the behavior of the self-service account recovery feature.
    /// </summary>
    public AccountRecoveryOptions AccountRecovery { get; set; }

    /// <summary>
    /// Options to control the behavior of the account verification feature.
    /// Note that the Cofoundry admin panel does not support an account 
    /// verification flow and therefore these settings do not apply.
    /// </summary>
    public AccountVerificationOptions AccountVerification { get; set; }

    /// <summary>
    /// Options to control the background task that runs to clean up 
    /// stale user data.
    /// </summary>
    public UserCleanupOptions Cleanup { get; set; }

    /// <summary>
    /// Create a new <see cref="UserAreaOptions"/>, copying data from 
    /// the specified <paramref name="settings"/>.
    /// </summary>
    /// <param name="settings">
    /// <see cref="UsersSettings"/> configuration to copy from.
    /// </param>
    public static UserAreaOptions CopyFrom(UsersSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.EmailAddress);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.Password);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.Username);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.Cookies);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.Authentication);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.AccountRecovery);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.AccountVerification);
        EntityInvalidOperationException.ThrowIfNull(settings, s => s.Cleanup);

        var options = new UserAreaOptions()
        {
            EmailAddress = settings.EmailAddress.Clone(),
            Password = settings.Password.Clone(),
            Username = settings.Username.Clone(),
            Cookies = settings.Cookies.Clone(),
            Authentication = settings.Authentication.Clone(),
            AccountRecovery = settings.AccountRecovery.Clone(),
            AccountVerification = settings.AccountVerification.Clone(),
            Cleanup = settings.Cleanup.Clone()
        };

        return options;
    }
}
