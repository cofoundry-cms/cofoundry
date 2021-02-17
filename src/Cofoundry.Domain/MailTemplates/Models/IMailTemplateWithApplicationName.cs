using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public interface IMailTemplateWithApplicationName
    {
        string ApplicationName { get; set; }
    }
}
