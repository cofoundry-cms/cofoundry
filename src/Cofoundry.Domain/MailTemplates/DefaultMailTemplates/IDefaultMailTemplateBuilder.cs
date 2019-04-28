using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Used construct the default mail templates that are used by Cofoundry 
    /// to send email for actions such as creating new users and resetting password 
    /// etc.
    /// </summary>
    public interface IDefaultMailTemplateBuilder<T>
        where T : IUserAreaDefinition
    {
        /// <summary>
        /// Creates a mail template that is used when a new user is
        /// created with a templorary password. This is typically
        /// when a user is created in the admin panel.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the temporary password.
        /// </param>
        Task<DefaultNewUserWithTemporaryPasswordMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(
            NewUserWithTemporaryPasswordTemplateBuilderContext context
            );

        /// <summary>
        /// Creates a mail template that is used when a user has their
        /// password reset by an administrator.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the temporary password.
        /// </param>
        Task<DefaultPasswordResetByAdminMailTemplate> BuildPasswordResetByAdminTemplateAsync(
            PasswordResetByAdminTemplateBuilderContext context
            );

        /// <summary>
        /// Creates a mail template that is used when a user requests to
        /// reset their password e.g. via a forgot password page.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the auth data to construct the reset url.
        /// </param>
        Task<DefaultPasswordResetRequestedByUserMailTemplate> BuildPasswordResetRequestedByUserTemplateAsync(
            PasswordResetRequestedByUserTemplateBuilderContext context
            );

        /// <summary>
        /// Creates a mail template that is used to send a notification
        /// when a user's password has been changed.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data.
        /// </param>
        Task<DefaultPasswordChangedMailTemplate> BuildPasswordChangedTemplateAsync(
            PasswordChangedTemplateBuilderContext context
            );
    }
}
