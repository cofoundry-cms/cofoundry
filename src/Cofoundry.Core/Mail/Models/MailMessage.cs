namespace Cofoundry.Core.Mail;

public class MailMessage
{
    public MailAddress To { get; set; }

    public MailAddress From { get; set; }

    public string Subject { get; set; }

    public string HtmlBody { get; set; }

    public string TextBody { get; set; }
}
