using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// A mail dispatch session can be used to send multiple
    /// emails while abstracting the lifetime management of the
    /// underlying resources. Sessions shouldn't have singleton 
    /// scope but should be opened on demand. You can influence
    /// batch sizes by using Flush, however it's up to the underlying
    /// implementation to determine how mail is batched and sent.
    /// </summary>
    public interface IMailDispatchSession : IDisposable
    {
        /// <summary>
        /// Adds a mail message to the queue of mail to be sent.
        /// </summary>
        /// <param name="mailMessage">The mail message to send.</param>
        void Add(MailMessage mailMessage);

        /// <summary>
        /// Dispatches any mail in the queue.
        /// </summary>
        Task FlushAsync();
    }
}
