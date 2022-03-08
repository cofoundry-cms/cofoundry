using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used to format 
    /// a "new user with temporary password" mail template.
    /// </summary>
    public interface INewUserWithTemporaryPasswordTemplateBuilderContext
    {
        /// <summary>
        /// The newly created user.
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
        Task<NewUserWithTemporaryPasswordMailTemplate> BuildDefaultTemplateAsync();
    }
}