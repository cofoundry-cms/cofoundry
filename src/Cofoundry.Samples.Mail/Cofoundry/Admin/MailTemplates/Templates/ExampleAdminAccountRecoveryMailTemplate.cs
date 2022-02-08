using Cofoundry.Core.Mail;

namespace Cofoundry.Samples.Mail.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests an account recovery 
    /// via the 'forgot password' mechanism on sign inpage. This version of the template
    /// is used by default for the Cofoundry admin user area.
    /// </summary>
    public class ExampleAdminAccountRecoveryMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Name or full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added.
        /// </summary>
        public string ViewFile { get; set; } = "~/Cofoundry/Admin/MailTemplates/Templates/ExampleAdminAccountRecoveryMailTemplate";

        /// <summary>
        /// String to use as the subject to the email. To customize this
        /// use the "SubjectFormat" property.
        /// </summary>
        public string Subject { get; } = "We've received a request to reset your password!";

        /// <summary>
        /// In our custom template we are including the first name field which 
        /// is not included in the standard Cofoundry template.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The username of the user requesting to recover their account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The absolute url for the complete account recovery form e.g.
        /// "https://example.com/admin/account-recovery-complete?t={token}".
        /// </summary>
        public string RecoveryUrl { get; set; }
    }
}