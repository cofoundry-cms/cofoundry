using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail;
using Cofoundry.Plugins.Mail.SendGrid.Internal;

namespace Cofoundry.Plugins.Mail.SendGrid.Registration;

public class SendGridDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        if (container.Configuration.GetValue<bool>("Cofoundry:Plugins:SendGrid:Disabled"))
        {
            return;
        }

        var overrideOptions = RegistrationOptions.Override();

        container
            .Register<IMailDispatchSession, SendGridMailDispatchSession>(overrideOptions)
            ;
    }
}
