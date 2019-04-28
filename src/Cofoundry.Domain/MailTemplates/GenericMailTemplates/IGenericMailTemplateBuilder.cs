using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public interface IGenericMailTemplateBuilder<T>
        where T : IUserAreaDefinition
    {
        Task<GenericNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
            NewUserWithTemporaryPasswordTemplateBuilderContext context
            );

        Task<GenericPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(
            PasswordResetByAdminTemplateBuilderContext context
            );

        Task<GenericPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(
            PasswordResetRequestedByUserTemplateBuilderContext context
            );

        Task<GenericPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(
            PasswordChangedTemplateBuilderContext context
            );
    }
}
