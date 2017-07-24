using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Service for sending emails. This service is the lower level of 
    /// dispatch, i.e. sending via SMTP or sendgrid.
    /// </summary>
    public interface IMailDispatchService
    {
        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        Task DispatchAsync(MailMessage message);

        /// <summary>
        /// Creates a new mail session that can be used to send batches of mail.
        /// </summary>
        /// <returns>New instance of an IMailDispatchSession</returns>
        IMailDispatchSession CreateSession();
    }
}
