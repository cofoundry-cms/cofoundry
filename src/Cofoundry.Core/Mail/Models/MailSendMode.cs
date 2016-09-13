using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Indictes the output method for sending emails.
    /// </summary>
    public enum MailSendMode
    {
        /// <summary>
        /// Do not send the mail, but save it to a local directory on the server.
        /// </summary>
        LocalDrop,

        /// <summary>
        /// Send the mail to the external recipient
        /// </summary>
        Send,

        /// <summary>
        /// Send the mail to a nominated address for debug purposes
        /// </summary>
        SendToDebugAddress,

        /// <summary>
        /// Do nothing, just mark the mail as sent.
        /// </summary>
        DoNotSend
    }
}
