using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class PasswordResetTemplateBuilderContext : IPasswordResetTemplateBuilderContext
{
    public required UserSummary User { get; set; }

    public required IHtmlContent TemporaryPassword { get; set; }

    public required Func<PasswordResetTemplateBuilderContext, Task<PasswordResetMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<PasswordResetMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
