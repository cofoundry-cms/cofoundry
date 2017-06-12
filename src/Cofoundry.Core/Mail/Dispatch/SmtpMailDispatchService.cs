using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Service for sending email via an SMTP server
    /// </summary>
    public class SmtpMailDispatchService:  IMailDispatchService
    {
        #region constructor

        private readonly MailSettings _mailSettings;
        private readonly SmtpMailSettings _smtpMailSettings;
        private readonly IPathResolver _pathResolver;
        
        public SmtpMailDispatchService(
            IPathResolver pathResolver,
            MailSettings mailSettings,
            SmtpMailSettings smtpMailSettings
            )
        {
            _mailSettings = mailSettings;
            _smtpMailSettings = smtpMailSettings;
            _pathResolver = pathResolver;
        }

        #endregion

        #region public methods

        public void Dispatch(MailMessage message, IMailClient mailClient = null)
        {
            if (_mailSettings.SendMode == MailSendMode.DoNotSend) return;

            if (mailClient != null)
            {
                ValidateMailClient(mailClient);
                SendMessage(message, (SmtpMailClient)mailClient);
            }
            else
            {
                using (var smtpClient = CreateSmtpMailClient())
                {
                    SendMessage(message, smtpClient);
                }
            }
        }
        
        public async Task DispatchAsync(MailMessage message, IMailClient mailClient = null)
        {
            if (_mailSettings.SendMode == MailSendMode.DoNotSend) return;

            if (mailClient != null)
            {
                ValidateMailClient(mailClient);
                await SendMessageAsync(message, (SmtpMailClient)mailClient);
            }
            else
            {
                using (var smtpClient = CreateSmtpMailClient())
                {
                    await SendMessageAsync(message, smtpClient);
                }
            }
        }

        public IMailClient CreateMailClient()
        {
            return CreateSmtpMailClient();
        }

        #endregion

        #region private methods

        private void SendMessage(MailMessage message, SmtpMailClient mailClient)
        {
            if (mailClient == null) throw new ArgumentNullException(nameof(mailClient));

            var messageToSend = FormatMessage(message);

            mailClient.Send(messageToSend);
        }

        private async Task SendMessageAsync(MailMessage message, SmtpMailClient mailClient)
        {
            if (mailClient == null) throw new ArgumentNullException(nameof(mailClient));

            var messageToSend = FormatMessage(message);

            await mailClient.SendAsync(messageToSend);
        }

        private void ValidateMailClient(IMailClient mailClient)
        {
            if (!(mailClient is SmtpMailClient))
            {
                throw new ArgumentException("SmtpMailDispatchService requires an IMailClient of type SmtpClient");
            }
        }

        private System.Net.Mail.MailMessage FormatMessage(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageToSend = new System.Net.Mail.MailMessage();

            MailAddress toAddress = GetMailToAddress(message);
            messageToSend.To.Add(toAddress);
            messageToSend.Subject = message.Subject;
            if (message.From != null)
            {
                messageToSend.From = message.From.ToMailAddress();
            }
            else
            {
                messageToSend.From = CreateMailAddress(_mailSettings.DefaultFromAddress, _mailSettings.DefaultFromAddressDisplayName);
            }
            SetMessageBody(messageToSend, message.HtmlBody, message.TextBody);

            return messageToSend;
        }

        private MailAddress GetMailToAddress(MailMessage message)
        {
            MailAddress toAddress;
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
                toAddress = message.To.ToMailAddress();
            }
            return toAddress;
        }

        private SmtpMailClient CreateSmtpMailClient()
        {
            var client = new SmtpClient();

            if (String.IsNullOrEmpty(client.Host))
            {
                client.Host = "localhost";
            }

            if (_mailSettings.SendMode == MailSendMode.LocalDrop)
            {
                client.EnableSsl = false;
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = GetDebugDropPath();
            }

            return new SmtpMailClient(client);
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
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content, Encoding.UTF8, mediaType));
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
            if (string.IsNullOrEmpty(_smtpMailSettings.DebugMailDropDirectory))
            {
                throw new Exception("Cofoundry:SmtpMail:DebugMailDropDirectory configuration has been requested and is not set.");
            }

            var debugMailDropDirectory = _pathResolver.MapPath(_smtpMailSettings.DebugMailDropDirectory);
            if (!Directory.Exists(debugMailDropDirectory))
            {
                Directory.CreateDirectory(debugMailDropDirectory);
            }

            return debugMailDropDirectory;
        }

        #endregion
    }
}
