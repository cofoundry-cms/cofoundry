using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used
    /// to format a password reset mail template
    /// </summary>
    /// <inheritdoc/>
    public interface IPasswordResetTemplateBuilderContext
    {
        /// <summary>
        /// The user that has had their password reset.
        /// </summary>
        UserSummary User { get; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        IHtmlContent TemporaryPassword { get; }

        /// <summary>
        /// Builds the default template, which you can optionally modify
        /// to your requirements.
        /// </summary>
        Task<PasswordResetMailTemplate> BuildDefaultTemplateAsync();
    }
}
