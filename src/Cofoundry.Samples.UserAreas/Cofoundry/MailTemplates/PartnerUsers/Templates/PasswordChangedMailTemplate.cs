using Cofoundry.Core.Mail;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// Template for the email sent to a partner user to notify then that
    /// their password has been changed.
    /// </summary>
    public class PasswordChangedMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordChangedMailTemplate));

        /// <summary>
        /// String to use as the subject to the email.
        /// </summary>
        public string Subject { get; } = "Your Password changed";

        /// <summary>
        /// The username of the user who has had their password changed.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute login page url e.g. "https://www.example.com/members/login".
        /// </summary>
        public string LoginUrl { get; set; }
    }
}