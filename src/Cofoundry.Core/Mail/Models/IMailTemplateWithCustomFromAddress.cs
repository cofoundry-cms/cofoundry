using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Represents an email template that includes a custom 'from' address
    /// </summary>
    public interface IMailTemplateWithCustomFromAddress : IMailTemplate
    {
        /// <summary>
        /// A custom email address to send the email from, which will override the default.
        /// </summary>
        MailAddress From { get; }
    }
}
