using Cofoundry.Core;
using Cofoundry.Core.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Cofoundry.Plugins.Mail.MailKit;

/// <summary>
/// MailKit implementation of <see cref="IMailDispatchSession"/>.
/// </summary>
public sealed class MailKitMailDispatchSession : IMailDispatchSession
{
    private readonly Queue<MimeMessage> _mailQueue = new();
    private readonly Lazy<SmtpClient?> _mailClient;
    private readonly MailSettings _mailSettings;
    private readonly IPathResolver _pathResolver;
    private readonly ISmtpClientConnectionConfiguration _smtpClientConnectionConfiguration;

    private bool _isDisposing;

    public MailKitMailDispatchSession(
        MailSettings mailSettings,
        IPathResolver pathResolver,
        ISmtpClientConnectionConfiguration smtpClientConnectionConfiguration
        )
    {
        _mailSettings = mailSettings;
        _pathResolver = pathResolver;
        _mailClient = new Lazy<SmtpClient?>(CreateSmtpMailClient);
        _smtpClientConnectionConfiguration = smtpClientConnectionConfiguration;
    }

    /// <inheritdoc/>
    public void Add(MailMessage mailMessage)
    {
        var messageToSend = FormatMessage(mailMessage);
        _mailQueue.Enqueue(messageToSend);
    }

    /// <inheritdoc/>
    public async Task FlushAsync()
    {
        ValidateNotDisposed();

        if (_mailSettings.SendMode == MailSendMode.LocalDrop)
        {
            FlushToLocalDrop();
            return;
        }

        var client = _mailClient.Value;

        if (client == null)
        {
            throw new InvalidOperationException($"SmtpClient should not be null, has {nameof(FlushAsync)} been called while disposing? {nameof(_isDisposing)}: {_isDisposing}");
        }

        try
        {
            await _smtpClientConnectionConfiguration.ConnectAsync(client);

            while (_mailQueue.Count > 0)
            {
                var mailItem = _mailQueue.Dequeue();
                if (mailItem != null && _mailSettings.SendMode != MailSendMode.DoNotSend)
                {
                    await client.SendAsync(mailItem);
                }
            }
        }
        finally
        {
            await _smtpClientConnectionConfiguration.DisconnectAsync(client);
        }
    }

    public void Dispose()
    {
        _isDisposing = true;
        if (_mailClient.IsValueCreated)
        {
            _mailClient.Value?.Dispose();
        }
    }

    private void ValidateNotDisposed()
    {
        if (_isDisposing)
        {
            throw new InvalidOperationException("Cannot perform the operation because the object has been disposed");
        }
    }

    /// <summary>
    /// see https://stackoverflow.com/a/39933156/716689
    /// </summary>
    private void FlushToLocalDrop()
    {
        var pickupDirectory = GetMailDropPath();

        while (_mailQueue.Count > 0)
        {
            var mailItem = _mailQueue.Dequeue();
            if (mailItem != null)
            {
                var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");

                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    mailItem.WriteTo(stream);
                    return;
                }
            }
        }
    }

    private SmtpClient? CreateSmtpMailClient()
    {
        if (_isDisposing)
        {
            return null;
        }

        return new SmtpClient();
    }

    private MimeMessage FormatMessage(MailMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageToSend = new MimeMessage();

        var toAddress = GetMailToAddress(message);
        messageToSend.To.Add(toAddress);
        messageToSend.Subject = message.Subject;
        if (message.From != null)
        {
            messageToSend.From.Add(CreateMailAddress(message.From.Address, message.From.DisplayName));
        }
        else
        {
            messageToSend.From.Add(CreateMailAddress(_mailSettings.DefaultFromAddress, _mailSettings.DefaultFromAddressDisplayName));
        }
        SetMessageBody(messageToSend, message.HtmlBody, message.TextBody);

        return messageToSend;
    }

    private MailboxAddress GetMailToAddress(MailMessage message)
    {
        MailboxAddress toAddress;
        if (_mailSettings.SendMode == MailSendMode.SendToDebugAddress)
        {
            if (string.IsNullOrEmpty(_mailSettings.DebugEmailAddress))
            {
                throw new Exception($"{nameof(MailSendMode)}.{nameof(MailSendMode.SendToDebugAddress)} requested but Cofoundry:Mail:DebugEmailAddress setting is not defined.");
            }
            toAddress = CreateMailAddress(_mailSettings.DebugEmailAddress, message.To.DisplayName);
        }
        else
        {
            toAddress = new MailboxAddress(message.To.DisplayName, message.To.Address);
        }
        return toAddress;
    }

    private static void SetMessageBody(MimeMessage message, string? bodyHtml, string? bodyText)
    {
        var hasHtmlBody = !string.IsNullOrWhiteSpace(bodyHtml);
        var hasTextBody = !string.IsNullOrWhiteSpace(bodyText);
        if (!hasHtmlBody && !hasTextBody)
        {
            throw new ArgumentException("An email must have either a html or text body");
        }

        if (hasHtmlBody && !hasTextBody)
        {
            message.Body = new TextPart(TextFormat.Html) { Text = bodyHtml };
        }
        else if (hasTextBody && !hasHtmlBody)
        {
            message.Body = new TextPart(TextFormat.Plain) { Text = bodyText };
        }
        else
        {
            var alternative = new Multipart("alternative")
            {
                new TextPart(TextFormat.Plain) { Text = bodyText },
                new TextPart(TextFormat.Html) { Text = bodyHtml }
            };

            message.Body = alternative;
        }
    }

    private static MailboxAddress CreateMailAddress(string email, string? displayName)
    {
        MailboxAddress? mailAddress;
        try
        {
            if (string.IsNullOrEmpty(displayName))
            {
                mailAddress = new MailboxAddress(null, email);
            }
            else
            {
                mailAddress = new MailboxAddress(displayName, email);
            }
        }
        catch (ParseException ex)
        {
            throw new InvalidMailAddressException(email, displayName, ex);
        }

        return mailAddress;
    }

    private string GetMailDropPath()
    {
        if (string.IsNullOrEmpty(_mailSettings.MailDropDirectory))
        {
            throw new Exception("Cofoundry:Mail:MailDropDirectory configuration has been requested and is not set.");
        }

        var mailDropDirectory = _pathResolver.MapPath(_mailSettings.MailDropDirectory);
        if (!Directory.Exists(mailDropDirectory))
        {
            Directory.CreateDirectory(mailDropDirectory);
        }

        return mailDropDirectory;
    }
}
