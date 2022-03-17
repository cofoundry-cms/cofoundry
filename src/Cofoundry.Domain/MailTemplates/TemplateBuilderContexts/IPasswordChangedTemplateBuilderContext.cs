namespace Cofoundry.Domain.MailTemplates;

/// <summary>
/// Context model containg data that can be used to format 
/// a "password changed" mail template.
/// </summary>
public interface IPasswordChangedTemplateBuilderContext
{
    /// <summary>
    /// The user that had their password changed.
    /// </summary>
    UserSummary User { get; }

    /// <summary>
    /// Builds the default template, which you can optionally modify
    /// to your requirements.
    /// </summary>
    Task<PasswordChangedMailTemplate> BuildDefaultTemplateAsync();
}
