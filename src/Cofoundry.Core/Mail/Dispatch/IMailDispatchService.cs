using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Service for sending emails. This service is the lower level of dispatch, i.e. sending via 
    /// SMTP or sendgrid.
    /// </summary>
    public interface IMailDispatchService
    {
        /// <summary>
        /// Sends a mail message, optionally using the specified mail client.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        /// <param name="mailClient">Optional IMailClient to use to send the message.</param>
        void Dispatch(MailMessage message, IMailClient mailClient = null);

        /// <summary>
        /// Sends a mail message, optionally using the specified mail client.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        /// <param name="mailClient">Optional IMailClient to use to send the message.</param>
        Task DispatchAsync(MailMessage message, IMailClient mailClient = null);

        /// <summary>
        /// Creates a new mail client which can be used to send batches of email.
        /// </summary>
        /// <returns>New instance of an IMailClient</returns>
        IMailClient CreateMailClient();
    }
}
