using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail.Internal
{
    /// <summary>
    /// This is a simple debug implementation that writes out mail
    /// to a text file to make debugging templates easier.
    /// </summary>
    public class DebugMailDispatchSession : IMailDispatchSession
    {
        #region constructor

        private readonly Queue<string> _mailQueue = new Queue<string>();
        private readonly MailSettings _mailSettings;
        private readonly IPathResolver _pathResolver;
        private readonly string _debugDropPath;

        public DebugMailDispatchSession(
            MailSettings mailSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _pathResolver = pathResolver;

            _debugDropPath = GetDebugDropPath();
        }

        #endregion

        #region public

        /// <summary>
        /// Formats and adds a mail message to the queue of mail to be sent.
        /// </summary>
        /// <param name="mailMessage">The mail message to send.</param>
        public void Add(MailMessage mailMessage)
        {
            var messageToSend = FormatMessage(mailMessage);
            _mailQueue.Enqueue(messageToSend);
        }

        /// <summary>
        /// Dispatches any mail in the queue.
        /// </summary>
        public async Task FlushAsync()
        {
            while (_mailQueue.Count > 0)
            {
                var mailItem = _mailQueue.Dequeue();
                if (mailItem != null && _mailSettings.SendMode != MailSendMode.DoNotSend)
                {
                    using (var writer = CreateFileStream())
                    {
                        await writer.WriteAsync(mailItem);
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        #endregion

        #region private methods

        private StreamWriter CreateFileStream()
        {
            var fileName = Path.Combine(_debugDropPath, Guid.NewGuid() + ".txt");
            return File.CreateText(fileName);
        }

        private string FormatMessage(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            ValidateMessageBody(message);

            var messageToSend = new StringBuilder();
            messageToSend.AppendLine("To: " + GetMailToAddress(message));
            messageToSend.AppendLine("From: " + GetMailFromAddress(message));
            messageToSend.AppendLine("Subject: " + message.Subject);

            messageToSend.AppendLine();
            messageToSend.AppendLine("=============== HTML ===============");
            messageToSend.AppendLine();

            messageToSend.AppendLine(message.HtmlBody);

            messageToSend.AppendLine();
            messageToSend.AppendLine("=============== TEXT ===============");
            messageToSend.AppendLine();

            messageToSend.AppendLine(message.TextBody);

            return messageToSend.ToString();
        }

        private string GetMailToAddress(MailMessage message)
        {
            string toAddress;
            if (_mailSettings.SendMode == MailSendMode.SendToDebugAddress)
            {
                if (string.IsNullOrEmpty(_mailSettings.DebugEmailAddress))
                {
                    throw new Exception("MailSendMode.SendToDebugAddress requested but Cofoundry:Mail:DebugEmailAddress setting is not defined.");
                }
                toAddress = CreateMailAddress(_mailSettings.DebugEmailAddress, message.To.DisplayName);
            }
            else
            {
                toAddress = CreateMailAddress(message.To.Address, message.To.DisplayName);
            }

            return toAddress;
        }

        private string GetMailFromAddress(MailMessage message)
        {
            string fromAddress;

            if (message.From != null)
            {
                fromAddress = CreateMailAddress(message.From.Address, message.From.DisplayName);
            }
            else
            {
                fromAddress = CreateMailAddress(_mailSettings.DefaultFromAddress, _mailSettings.DefaultFromAddressDisplayName);
            }

            return fromAddress;
        }

        private void ValidateMessageBody(MailMessage message)
        {
            var hasHtmlBody = !string.IsNullOrWhiteSpace(message.HtmlBody);
            var hasTextBody = !string.IsNullOrWhiteSpace(message.TextBody);

            if (!hasHtmlBody && !hasTextBody)
            {
                throw new FormatException("An email must have either a html or text body");
            }
        }

        private string CreateMailAddress(string email, string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return email;
            }

            return displayName + $"({email})";
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
