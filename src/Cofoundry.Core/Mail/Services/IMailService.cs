using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// A service for sending email communications.
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        void Send(string toEmail, string displayName, IMailTemplate template);

        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        Task SendAsync(string toEmail, string toDisplayName, IMailTemplate template);

        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        void Send(string toEmail, IMailTemplate template);

        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        Task SendAsync(string toEmail, IMailTemplate template);

        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        void Send(SerializeableMailAddress address, IMailTemplate template);

        /// <summary>
        /// Sends an email to the specified email address
        /// </summary>
        Task SendAsync(SerializeableMailAddress address, IMailTemplate template);
    }
}
