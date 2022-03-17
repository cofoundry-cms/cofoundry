namespace Cofoundry.Core.Mail;

/// <summary>
/// A service for sending email communications.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends an email to the specified email address
    /// </summary>
    Task SendAsync(string toEmail, string toDisplayName, IMailTemplate template);

    /// <summary>
    /// Sends an email to the specified email address
    /// </summary>
    Task SendAsync(string toEmail, IMailTemplate template);

    /// <summary>
    /// Sends an email to the specified email address
    /// </summary>
    Task SendAsync(MailAddress address, IMailTemplate template);
}
