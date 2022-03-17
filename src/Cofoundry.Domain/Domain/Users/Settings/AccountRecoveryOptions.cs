using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Core.Web;

namespace Cofoundry.Domain;

/// <summary>
/// Options to control the behavior of the self-service account recovery feature.
/// </summary>
public class AccountRecoveryOptions : IValidatableObject
{
    public AccountRecoveryOptions()
    {
    }

    /// <summary>
    /// The length of time an account recovery token is valid for, specified as a 
    /// <see cref="TimeSpan"/> or in JSON configuration as a time format string 
    /// e.g. "01:00:00" to represent 1 hour. Defaults to 16 hours. If zero or
    /// less, then time-based validation does not occur.
    /// </summary>
    public TimeSpan ExpireAfter { get; set; } = TimeSpan.FromHours(16);

    /// <summary>
    /// The maximum number of account recovery initiation attempts to allow per IP address. The 
    /// default value is 16 attempts per day.
    /// </summary>
    [ValidateObject]
    public RateLimitConfiguration InitiationRateLimit { get; set; } = new RateLimitConfiguration(16, TimeSpan.FromDays(1));

    /// <summary>
    /// The randomized duration parameters when executing the account recovery (forgot password) initiation 
    /// command. The lower bound of the duration should exceed the expected execution time of the account recovery 
    /// initialization command to mitigate time-based enumeration attacks to discover valid usernames.
    /// Defaults to a random duration between 1.5 and 2 seconds.
    /// </summary>
    [ValidateObject]
    public RandomizedExecutionDuration ExecutionDuration { get; set; } = new RandomizedExecutionDuration()
    {
        MinInMilliseconds = 1500,
        MaxInMilliseconds = 2000
    };

    /// <summary>
    /// <para>
    /// The relative base path used to construct the URL for the account recovery completion
    /// form. A unique token will be added as a query parameter to the URL, it is then
    /// resolved using <see cref="ISiteUrlResolver.MakeAbsolute(string)"/> and added to the 
    /// email notification e.g. "/auth/account-recovery/complete" would be transformed to 
    /// "https://example.com/auth/account-recovery/complete?t={token}". The path can include
    /// other query parameters, which will be merged into the resulting url.
    /// </para>
    /// <para>
    /// This setting is required when using the account recovery feature, unless you are building
    /// the url yourself in a custom <see cref="MailTemplates.DefaultMailTemplates.IDefaultMailTemplateBuilder{T}"/> 
    /// implementation. Changing this setting does not affect the Cofoundry Admin account recovery feature.
    /// </para>
    /// </summary>
    public string RecoveryUrlBase { get; set; }

    /// <summary>
    /// Copies the options to a new instance, which can be modified
    /// without altering the base settings. This is used for user area
    /// specific configuration.
    /// </summary>
    public AccountRecoveryOptions Clone()
    {
        return new AccountRecoveryOptions()
        {
            ExpireAfter = ExpireAfter,
            InitiationRateLimit = InitiationRateLimit.Clone(),
            RecoveryUrlBase = RecoveryUrlBase
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RecoveryUrlBase != null && !Uri.IsWellFormedUriString(RecoveryUrlBase, UriKind.Relative))
        {
            yield return new ValidationResult($"{RecoveryUrlBase} must be a relative url.", new string[] { nameof(RecoveryUrlBase) });
        }
    }
}
