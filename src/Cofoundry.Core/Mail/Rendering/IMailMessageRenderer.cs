using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Renders the contents of a mail template to html/text strings in preparation for 
    /// sending out as an email.
    /// </summary>
    public interface IMailMessageRenderer
    {
        /// <summary>
        ///  Renders the contents of a mail template and formats it into a MailMessage
        ///  object that can be used to send out an email.
        /// </summary>
        /// <param name="template">The mail template that describes the data and template information for the email</param>
        /// <param name="toAddress">The address to send the email to.</param>
        /// <returns>Formatted MailMessage</returns>
        Task<MailMessage> RenderAsync(IMailTemplate template, MailAddress toAddress);
    }
}
