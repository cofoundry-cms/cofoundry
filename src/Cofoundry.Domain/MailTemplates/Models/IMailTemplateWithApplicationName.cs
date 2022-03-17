using Cofoundry.Core.Mail;

namespace Cofoundry.Domain.MailTemplates;

/// <summary>
/// Used to mark up a template that contains an application name. In
/// the default Cofoundry templates this is used in the layout to
/// conditionally write out the app name in the footer.
/// </summary>
public interface IMailTemplateWithApplicationName : IMailTemplate
{
    /// <summary>
    /// The name of the application sending the email.
    /// </summary>
    string ApplicationName { get; set; }
}
