using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Handles a connection to a client that allows the sending of
    /// mail messages.
    /// </summary>
    public interface IMailClient : IDisposable
    {
        void Send(System.Net.Mail.MailMessage msg);
        Task SendAsync(System.Net.Mail.MailMessage msg);
    }
}
