namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class AccountVerificationTemplateBuilderContext : IAccountVerificationTemplateBuilderContext
{
    public required UserSummary User { get; set; }

    public required string Token { get; set; }

    public string VerificationUrlPath { get; set; } = string.Empty;

    public required Func<AccountVerificationTemplateBuilderContext, Task<AccountVerificationMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<AccountVerificationMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
