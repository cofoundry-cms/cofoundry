using Cofoundry.Core.Mail;

namespace SimpleSite;

/// <summary>
/// Cofoundry includes a framework for sending mail based around template
/// classes and view files rendered using RazorEngine.
/// 
/// For more information see https://www.cofoundry.org/docs/framework/mail
/// </summary>
public class ContactRequestMailTemplate : IMailTemplate
{
    public string ViewFile
    {
        get
        {
            return "~/Views/EmailTemplates/ContactRequest";
        }
    }

    public string Subject
    {
        get
        {
            return "New Contact Request";
        }
    }

    public required ContactRequest Request { get; set; }
}
