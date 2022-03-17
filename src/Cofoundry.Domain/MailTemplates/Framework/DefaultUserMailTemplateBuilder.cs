using Cofoundry.Core.Mail;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class DefaultUserMailTemplateBuilder<T> : IDefaultUserMailTemplateBuilder<T>
    where T : IUserAreaDefinition
{
    public virtual async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
        INewUserWithTemporaryPasswordTemplateBuilderContext context
        )
    {
        return await context.BuildDefaultTemplateAsync();
    }

    public virtual async Task<IMailTemplate> BuildPasswordResetTemplateAsync(IPasswordResetTemplateBuilderContext context)
    {
        return await context.BuildDefaultTemplateAsync();
    }

    public virtual async Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(IAccountRecoveryTemplateBuilderContext context)
    {
        return await context.BuildDefaultTemplateAsync();
    }

    public virtual async Task<IMailTemplate> BuildAccountVerificationTemplateAsync(IAccountVerificationTemplateBuilderContext context)
    {
        return await context.BuildDefaultTemplateAsync();
    }

    public virtual async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(IPasswordChangedTemplateBuilderContext context)
    {
        return await context.BuildDefaultTemplateAsync();
    }
}
