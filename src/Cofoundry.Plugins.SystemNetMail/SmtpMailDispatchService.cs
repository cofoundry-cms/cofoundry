using Cofoundry.Core;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.SystemNetMail
{
    /// <summary>
    /// Service for sending email via an SMTP server using 
    /// System.Net.Mail.
    /// </summary>
    public class SmtpMailDispatchService:  IMailDispatchService
    {
        #region constructor

        private readonly MailSettings _mailSettings;
        private readonly IPathResolver _pathResolver;
        
        public SmtpMailDispatchService(
            IPathResolver pathResolver,
            MailSettings mailSettings
            )
        {
            _mailSettings = mailSettings;
            _pathResolver = pathResolver;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public void Dispatch(MailMessage message)
        {
            using (var session = CreateSession())
            {
                session.Add(message);
                session.Flush();
            }
        }

        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public async Task DispatchAsync(MailMessage message)
        {
            using (var session = CreateSession())
            {
                session.Add(message);
                await session.FlushAsync();
            }
        }

        /// <summary>
        /// Creates a new mail session that can be used to send batches of mail.
        /// </summary>
        /// <returns>New instance of an IMailDispatchSession</returns>
        public IMailDispatchSession CreateSession()
        {
            return new SmtpMailDispatchSession(_mailSettings, _pathResolver);
        }

        #endregion
    }
}
