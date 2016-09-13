using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public interface IMailMessageRenderer
    {
        MailMessage Render(IMailTemplate template, SerializeableMailAddress toAddress);
    }
}
