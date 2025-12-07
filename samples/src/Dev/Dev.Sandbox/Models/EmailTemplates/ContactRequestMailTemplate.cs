using Cofoundry.Core.Mail;

namespace Dev.Sandbox;

public class ContactRequestMailTemplate : IMailTemplate
{
    public string ViewFile => "~/Views/EmailTemplates/ContactRequest";

    public string Subject => "New Contact Request";

    public required ContactRequest Request { get; set; }
}
