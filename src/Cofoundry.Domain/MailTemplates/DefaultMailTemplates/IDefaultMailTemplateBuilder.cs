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
        /// Creates a mail template that is used when a new user is created with a 
        /// temporary password. This is typically used when a user is not available 
        /// to provide their own password e.g. when a user is created in the admin panel.
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
        Task<DefaultPasswordResetMailTemplate> BuildPasswordResetTemplateAsync(
            PasswordResetTemplateBuilderContext context
            );

        /// <summary>
        /// Creates a mail template that is used when a user initiates an account
        /// recovery flow e.g. via a forgot password page.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the recovery url.
        /// </param>
        Task<DefaultAccountRecoveryMailTemplate> BuildAccountRecoveryTemplateAsync(
            AccountRecoveryTemplateBuilderContext context
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
