using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail.Internal;

namespace Cofoundry.Core.Mail.Registration;

public class MailDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IMailService, SimpleMailService>()
            .Register<IMailMessageRenderer, MailMessageRenderer>()
            .Register<IMailViewRenderer, RazorMailViewRenderer>()
            .Register<IMailDispatchService, DefaultMailDispatchService>()
            .Register<IMailDispatchSession, DebugMailDispatchSession>(RegistrationOptions.TransientScope())
            .Register<IMailDispatchSessionFactory, DefaultMailDispatchSessionFactory>()
            ;
    }
}
