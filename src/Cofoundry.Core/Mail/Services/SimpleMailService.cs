using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail.Internal
{
    /// <summary>
    /// A service to for sending email directly using the IMailClient implementation. Does not support
    /// batch (high volumne), queuing or background sending.
    /// </summary>
    public class SimpleMailService : IMailService
    {
        #region constructor

        private readonly IMailDispatchService _mailDispatchService;
        private readonly IMailMessageRenderer _mailMessageRenderer;

        public SimpleMailService(
            IMailDispatchService mailDispatchService,
            IMailMessageRenderer mailMessageRenderer
            )
        {
            _mailDispatchService = mailDispatchService;
            _mailMessageRenderer = mailMessageRenderer;
        }

        #endregion

        #region public methods

        public async Task SendAsync(string toEmail, string toDisplayName, IMailTemplate template)
        {
            var toAddress = new MailAddress(toEmail, toDisplayName);
            var message = await _mailMessageRenderer.RenderAsync(template, toAddress);

            await _mailDispatchService.DispatchAsync(message);
        }

        public async Task SendAsync(string toEmail, IMailTemplate template)
        {
            await SendAsync(toEmail, null, template);
        }

        public async Task SendAsync(MailAddress address, IMailTemplate template)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));

            await SendAsync(address.Address, address.DisplayName, template);
        }

        #endregion
    }
}
