using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Mail;

namespace Cofoundry.Core.Mail
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

        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public void Send(string toEmail, string toDisplayName, IMailTemplate template)
        {
            var toAddress = new SerializeableMailAddress(toEmail, toDisplayName);
            var message = _mailMessageRenderer.Render(template, toAddress);

            _mailDispatchService.Dispatch(message);
        }


        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public Task SendAsync(string toEmail, string toDisplayName, IMailTemplate template)
        {
            var toAddress = new SerializeableMailAddress(toEmail, toDisplayName);
            var message = _mailMessageRenderer.Render(template, toAddress);

            return _mailDispatchService.DispatchAsync(message);
        }

        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public void Send(string toEmail, IMailTemplate template)
        {
            Send(toEmail, null, template);
        }

        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public async Task SendAsync(string toEmail, IMailTemplate template)
        {
            await SendAsync(toEmail, null, template);
        }

        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public void Send(SerializeableMailAddress address, IMailTemplate template)
        {
            Send(address.Address, address.DisplayName, template);
        }

        /// <summary>
        /// Queues an email for sending to the specified email address
        /// </summary>
        public async Task SendAsync(SerializeableMailAddress address, IMailTemplate template)
        {
            Condition.Requires(address).IsNotNull();

            await SendAsync(address.Address, address.DisplayName, template);
        }

        #endregion
    }
}
