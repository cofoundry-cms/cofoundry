using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using Cofoundry.Core.Mail;

namespace Cofoundry.Plugins.SystemNetMail
{
    public class SystemNetMailDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var overrideOptions = RegistrationOptions.Override();

            container
                .RegisterType<IMailDispatchService, SmtpMailDispatchService>(overrideOptions)
                ; 
        }
    }
}
