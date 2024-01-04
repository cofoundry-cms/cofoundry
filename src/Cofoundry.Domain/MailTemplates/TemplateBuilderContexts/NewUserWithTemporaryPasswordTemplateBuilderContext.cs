using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class NewUserWithTemporaryPasswordTemplateBuilderContext : INewUserWithTemporaryPasswordTemplateBuilderContext
{
    public required UserSummary User { get; set; }

    public required IHtmlContent TemporaryPassword { get; set; }

    public required Func<NewUserWithTemporaryPasswordTemplateBuilderContext, Task<NewUserWithTemporaryPasswordMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<NewUserWithTemporaryPasswordMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
