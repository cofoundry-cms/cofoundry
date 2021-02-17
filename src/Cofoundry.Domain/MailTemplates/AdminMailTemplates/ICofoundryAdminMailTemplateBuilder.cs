using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public interface ICofoundryAdminMailTemplateBuilder
    {
        Task<AdminNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context);

        Task<AdminPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(PasswordResetByAdminTemplateBuilderContext context);

        Task<AdminPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(PasswordResetRequestedByUserTemplateBuilderContext context);

        Task<AdminPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context);
    }
}
