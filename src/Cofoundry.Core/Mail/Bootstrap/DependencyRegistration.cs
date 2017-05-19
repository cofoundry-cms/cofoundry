using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
//using RazorEngine.Templating;

namespace Cofoundry.Core.Mail
{
    public class MailDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IMailService, SimpleMailService>()
                .RegisterType<IMailDispatchService, SmtpMailDispatchService>()
                .RegisterType<IMailMessageRenderer, MailMessageRenderer>()
                .RegisterType<IMailViewRenderer, RazorMailViewRenderer>()
                ; 
        }
    }
}
