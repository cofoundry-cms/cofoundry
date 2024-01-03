namespace Cofoundry.Core.Mail;

public class MailMessage
{
    public required MailAddress To { get; set; }

    public MailAddress? From { get; set; }

    public required string Subject { get; set; }

    public string? HtmlBody { get; set; }

    public string? TextBody { get; set; }
}
