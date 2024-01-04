namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class PasswordChangedTemplateBuilderContext : IPasswordChangedTemplateBuilderContext
{
    public required UserSummary User { get; set; }

    public required Func<PasswordChangedTemplateBuilderContext, Task<PasswordChangedMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<PasswordChangedMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
