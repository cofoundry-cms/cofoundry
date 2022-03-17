using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class NewUserWithTemporaryPasswordTemplateBuilderContext : INewUserWithTemporaryPasswordTemplateBuilderContext
{
    public UserSummary User { get; set; }

    public IHtmlContent TemporaryPassword { get; set; }

    public Func<NewUserWithTemporaryPasswordTemplateBuilderContext, Task<NewUserWithTemporaryPasswordMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<NewUserWithTemporaryPasswordMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
