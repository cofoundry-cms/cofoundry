using Cofoundry.Core.Mail;

namespace SPASite.Domain;

/// <summary>
/// Cofoundry includes a framework for sending mail based around template
/// classes and razor view files.
/// 
/// For more information see https://www.cofoundry.org/docs/framework/mail
/// </summary>
public class NewUserWelcomeMailTemplate : IMailTemplate
{
    public string ViewFile
    {
        get { return "~/MailTemplates/NewUserWelcomeMail"; }
    }

    public string Subject
    {
        get { return "Welcome to SPA Cats!"; }
    }

    public string Name { get; set; } = string.Empty;
}
