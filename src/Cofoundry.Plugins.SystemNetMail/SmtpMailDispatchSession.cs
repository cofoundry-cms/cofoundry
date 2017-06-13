using Cofoundry.Core;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.SystemNetMail
{
    /// <summary>
    /// Mail dispatch session that uses System.Net.Mail to
    /// dispatch email.
    /// </summary>
    public class SmtpMailDispatchSession : IMailDispatchSession
    {
        private readonly Queue<System.Net.Mail.MailMessage> _mailQueue = new Queue<System.Net.Mail.MailMessage>();
        private readonly Lazy<System.Net.Mail.SmtpClient> _mailClient;
        private readonly MailSettings _mailSettings;
        private readonly IPathResolver _pathResolver;

        private bool isDisposing = false;

        public SmtpMailDispatchSession(
            MailSettings mailSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _pathResolver = pathResolver;
            _mailClient = new Lazy<System.Net.Mail.SmtpClient>(CreateSmtpMailClient);
        }

        public void Add(MailMessage mailMessage)
        {
            var messageToSend = FormatMessage(mailMessage);
            _mailQueue.Enqueue(messageToSend);
        }

        public void Flush()
        {
            while (_mailQueue.Count > 0)
            {
                var mailItem = _mailQueue.Dequeue();
                if (mailItem != null && _mailSettings.SendMode != MailSendMode.DoNotSend)
                {
                    _mailClient.Value.Send(mailItem);
                }
            }
        }

        public async Task FlushAsync()
        {
            while (_mailQueue.Count > 0)
            {
                var mailItem = _mailQueue.Dequeue();
                if (mailItem != null && _mailSettings.SendMode != MailSendMode.DoNotSend)
                {
                    await _mailClient.Value?.SendMailAsync(mailItem);
                }
            }
        }

        public void Dispose()
        {
            isDisposing = true;
            if (_mailClient.IsValueCreated)
            {
                _mailClient.Value?.Dispose();
            }
        }

        #region private methods
        
        private System.Net.Mail.SmtpClient CreateSmtpMailClient()
        {
            if (isDisposing) return null;

            var client = new System.Net.Mail.SmtpClient();

            if (String.IsNullOrEmpty(client.Host))
            {
                client.Host = "localhost";
            }

            if (_mailSettings.SendMode == MailSendMode.LocalDrop)
            {
                client.EnableSsl = false;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = GetDebugDropPath();
            }

            return client;
        }
        
        private System.Net.Mail.MailMessage FormatMessage(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageToSend = new System.Net.Mail.MailMessage();

            System.Net.Mail.MailAddress toAddress = GetMailToAddress(message);
            messageToSend.To.Add(toAddress);
            messageToSend.Subject = message.Subject;
            if (message.From != null)
            {
                messageToSend.From = new System.Net.Mail.MailAddress(message.From.Address, message.From.DisplayName);
            }
            else
            {
                messageToSend.From = CreateMailAddress(_mailSettings.DefaultFromAddress, _mailSettings.DefaultFromAddressDisplayName);
            }
            SetMessageBody(messageToSend, message.HtmlBody, message.TextBody);

            return messageToSend;
        }

        private System.Net.Mail.MailAddress GetMailToAddress(MailMessage message)
        {
            System.Net.Mail.MailAddress toAddress;
            if (_mailSettings.SendMode == MailSendMode.SendToDebugAddress)
            {
                if (string.IsNullOrEmpty(_mailSettings.DebugEmailAddress))
                {
                    throw new Exception("MailSendMode.SendToDebugAddress requested but Cofoundry:SmtpMail:DebugEmailAddress setting is not defined.");
                }
                toAddress = CreateMailAddress(_mailSettings.DebugEmailAddress, message.To.DisplayName);
            }
            else
            {
                toAddress = new System.Net.Mail.MailAddress(message.To.Address, message.To.DisplayName);
            }
            return toAddress;
        }

        private void SetMessageBody(System.Net.Mail.MailMessage message, string bodyHtml, string bodyText)
        {
            var hasHtmlBody = !string.IsNullOrWhiteSpace(bodyHtml);
            var hasTextBody = !string.IsNullOrWhiteSpace(bodyText);
            if (!hasHtmlBody && !hasTextBody)
            {
                throw new ArgumentException("An email must have either a html or text body");
            }

            message.BodyEncoding = Encoding.UTF8;

            if (hasHtmlBody && !hasTextBody)
            {
                message.Body = bodyHtml;
                message.IsBodyHtml = true;
            }
            else if (hasTextBody && !hasHtmlBody)
            {
                message.Body = bodyText;
                message.IsBodyHtml = false;
            }
            else
            {
                AddAlternateView(message, bodyText, MediaTypeNames.Text.Plain);
                AddAlternateView(message, bodyHtml, MediaTypeNames.Text.Html);
            }
        }

        private void AddAlternateView(System.Net.Mail.MailMessage message, string content, string mediaType)
        {
            message.AlternateViews.Add(System.Net.Mail.AlternateView.CreateAlternateViewFromString(content, Encoding.UTF8, mediaType));
        }

        private System.Net.Mail.MailAddress CreateMailAddress(string email, string displayName)
        {
            System.Net.Mail.MailAddress mailAddress = null;
            try
            {
                if (string.IsNullOrEmpty(displayName))
                {
                    mailAddress = new System.Net.Mail.MailAddress(email);
                }
                else
                {
                    mailAddress = new System.Net.Mail.MailAddress(email, displayName);
                }
            }
            catch (System.FormatException ex)
            {
                throw new Exception(string.Format("Invalid mail address: {0}, display name: {1}", email, displayName), ex);
            }

            return mailAddress;
        }

        private string GetDebugDropPath()
        {
            if (string.IsNullOrEmpty(_mailSettings.MailDropDirectory))
            {
                throw new Exception("Cofoundry:Mail:MailDropDirectory configuration has been requested and is not set.");
            }

            var debugMailDropDirectory = _pathResolver.MapPath(_mailSettings.MailDropDirectory);
            if (!Directory.Exists(debugMailDropDirectory))
            {
                Directory.CreateDirectory(debugMailDropDirectory);
            }

            return debugMailDropDirectory;
        }

        #endregion
    }
}
