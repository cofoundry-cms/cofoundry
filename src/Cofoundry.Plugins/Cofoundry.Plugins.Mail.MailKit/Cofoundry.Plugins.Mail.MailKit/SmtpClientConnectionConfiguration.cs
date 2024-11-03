using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Cofoundry.Core.Configuration;
using MailKit.Net.Smtp;

namespace Cofoundry.Plugins.Mail.MailKit;

/// <summary>
/// Default implementation of <see cref="SmtpClientConnectionConfiguration"/>.
/// </summary>
public class SmtpClientConnectionConfiguration : ISmtpClientConnectionConfiguration
{
    private readonly MailKitSettings _mailKitSettings;

    public SmtpClientConnectionConfiguration(
        MailKitSettings mailKitSettings
        )
    {
        _mailKitSettings = mailKitSettings;
    }

    /// <inheritdoc/>
    public virtual void Initialize(SmtpClient smtpClient)
    {
        ArgumentNullException.ThrowIfNull(smtpClient);

        switch (_mailKitSettings.CertificateValidationMode)
        {
            case CertificateValidationMode.All:
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                break;
            case CertificateValidationMode.ValidOnly:
                smtpClient.ServerCertificateValidationCallback = ValidateValidCertificatesOnly;
                break;
            case CertificateValidationMode.Default:
                break;
            default:
                throw new InvalidConfigurationException(nameof(_mailKitSettings.CertificateValidationMode), "Unknown CertificateValidationMode.");
        }
    }

    /// <inheritdoc/>
    public virtual void Connect(SmtpClient smtpClient)
    {
        ArgumentNullException.ThrowIfNull(smtpClient);

        if (smtpClient.IsConnected)
        {
            return;
        }

        smtpClient.Connect(_mailKitSettings.Host, _mailKitSettings.Port, _mailKitSettings.EnableSsl);

        if (!string.IsNullOrWhiteSpace(_mailKitSettings.UserName) && !smtpClient.IsAuthenticated)
        {
            smtpClient.Authenticate(_mailKitSettings.UserName, _mailKitSettings.Password);
        }
    }

    /// <inheritdoc/>
    public virtual async Task ConnectAsync(SmtpClient smtpClient)
    {
        ArgumentNullException.ThrowIfNull(smtpClient);

        if (smtpClient.IsConnected)
        {
            return;
        }

        await smtpClient.ConnectAsync(_mailKitSettings.Host, _mailKitSettings.Port, _mailKitSettings.EnableSsl);

        if (!string.IsNullOrWhiteSpace(_mailKitSettings.UserName))
        {
            await smtpClient.AuthenticateAsync(_mailKitSettings.UserName, _mailKitSettings.Password);
        }
    }

    /// <inheritdoc/>
    public virtual void Disconnect(SmtpClient smtpClient)
    {
        ArgumentNullException.ThrowIfNull(smtpClient);

        smtpClient.Disconnect(true);
    }

    /// <inheritdoc/>
    public virtual Task DisconnectAsync(SmtpClient smtpClient)
    {
        ArgumentNullException.ThrowIfNull(smtpClient);

        return smtpClient.DisconnectAsync(true);
    }

    private static bool ValidateValidCertificatesOnly(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == SslPolicyErrors.None;
    }
}
