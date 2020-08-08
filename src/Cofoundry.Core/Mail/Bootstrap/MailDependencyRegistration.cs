using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.Mail.DependencyRegistration
{
    public class MailDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IMailService, SimpleMailService>()
                .Register<IMailMessageRenderer, MailMessageRenderer>()
                .Register<IMailViewRenderer, RazorMailViewRenderer>()
                .Register<IMailDispatchService, DebugMailDispatchService>()
                ; 
        }
    }
}
