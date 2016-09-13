using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// IMailClient wrapper for SmtpClient
    /// </summary>
    public class SmtpMailClient : IMailClient
    {
        private readonly SmtpClient _client;

        public SmtpMailClient(SmtpClient client)
        {
            _client = client;
        }

        public void Send(System.Net.Mail.MailMessage msg)
        {
            _client.Send(msg);
        }

        public async Task SendAsync(System.Net.Mail.MailMessage msg)
        {
            await _client.SendMailAsync(msg);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
