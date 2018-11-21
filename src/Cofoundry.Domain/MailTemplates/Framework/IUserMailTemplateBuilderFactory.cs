using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public interface IUserMailTemplateBuilderFactory
    {
        IUserMailTemplateBuilder Create(string userAreaDefinitionCode);
    }
}
