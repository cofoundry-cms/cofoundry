using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail.Internal;

namespace Cofoundry.Core.Mail.Registration
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
