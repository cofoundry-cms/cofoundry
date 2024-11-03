using MailKit.Net.Smtp;

namespace Cofoundry.Plugins.Mail.MailKit;

/// <summary>
/// Used to configure the MailKit SmtpClient and customize the connection process.
/// </summary>
public interface ISmtpClientConnectionConfiguration
{
    /// <summary>
    /// Initialized the SmtpClient after it has been created.
    /// </summary>
    /// <param name="smtpClient">Instance to initialize.</param>
    void Initialize(SmtpClient smtpClient);

    /// <summary>
    /// Opens the SmtpClient connection to the configured host.
    /// </summary>
    /// <param name="smtpClient">Instance to connect with.</param>
    void Connect(SmtpClient smtpClient);

    /// <summary>
    /// Opens the SmtpClient connection to the configured host.
    /// </summary>
    /// <param name="smtpClient">Instance to connect with.</param>
    Task ConnectAsync(SmtpClient smtpClient);

    /// <summary>
    /// Closes the SmtpClient connection to the configured host.
    /// </summary>
    /// <param name="smtpClient">Instance to close the connection for.</param>
    void Disconnect(SmtpClient smtpClient);

    /// <summary>
    /// Closes the SmtpClient connection to the configured host.
    /// </summary>
    /// <param name="smtpClient">Instance to close the connection for.</param>
    Task DisconnectAsync(SmtpClient smtpClient);
}
