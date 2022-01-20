using Cofoundry.Core.Mail;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Indicates a class that is used construct mail templates that are
    /// used by Cofoundry to send email for actions such as creating new 
    /// users and resetting password etc. If customizing templates for a
    /// specific user area you need to implement the generic version of 
    /// this interface.
    /// </summary>
    public interface IUserMailTemplateBuilder
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
        Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(NewUserWithTemporaryPasswordTemplateBuilderContext context);

        /// <summary>
        /// Creates a mail template that is used when a user has their
        /// password reset by an administrator.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the temporary password.
        /// </param>
        Task<IMailTemplate> BuildPasswordResetTemplateAsync(PasswordResetTemplateBuilderContext context);

        /// <summary>
        /// Creates a mail template that is used when a user initiates an account
        /// recovery flow e.g. via a forgot password page.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data
        /// and the recovery url.
        /// </param>
        Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(AccountRecoveryTemplateBuilderContext context);

        /// <summary>
        /// Creates a mail template that is used to send a notification
        /// when a user's password has been changed.
        /// </summary>
        /// <param name="context">
        /// Key data that can be used in the template such as user data.
        /// </param>
        Task<IMailTemplate> BuildPasswordChangedTemplateAsync(PasswordChangedTemplateBuilderContext context);
    }

    /// <summary>
    /// <para>
    /// Indicates a class that is used construct mail templates that are
    /// used by Cofoundry to send email for actions such as creating new 
    /// users and resetting password etc. 
    /// </para>
    /// <para>
    /// Implement this generic version of the interface if you want to
    /// override the default mail templates for a specific user area.
    /// </para>
    /// </summary>
    public interface IUserMailTemplateBuilder<TUserAreaDefinition>
        : IUserMailTemplateBuilder
        where TUserAreaDefinition : IUserAreaDefinition
    {
    }
}
