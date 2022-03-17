namespace Cofoundry.Domain.MailTemplates;

/// <summary>
/// Context model containg data that can be used to format 
/// an account recovery (AKA forgot password) mail template.
/// </summary>
public interface IAccountRecoveryTemplateBuilderContext
{
    /// <summary>
    /// The user who is requesting to recover their account.
    /// </summary>
    UserSummary User { get; }

    /// <summary>
    /// A token used to identify and authenticate when recovering the account. This
    /// token has been used to create the <see cref="RecoveryUrlPath"/> property,
    /// but is provided separately here so you can rebuild your own URL if required.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// The relative URL for the account recovery completion form including the token 
    /// parameter e.g. "/auth/recover-account?t=example-token". Generally  this should 
    /// not be <see langword="null"/> because it should be configured in <see cref="AccountRecoveryOptions.RecoveryUrlBase"/>,
    /// however, it is possible to be <see langword="null"/> if this setting was not 
    /// set or the URL building is handled in a custom <see cref="IUserMailTemplateBuilder{T}"/> 
    /// implementation.
    /// </summary>
    string RecoveryUrlPath { get; }

    /// <summary>
    /// Builds the default template, which you can optionally modify
    /// to your requirements.
    /// </summary>
    Task<AccountRecoveryMailTemplate> BuildDefaultTemplateAsync();
}
