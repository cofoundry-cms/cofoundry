using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail;

namespace Cofoundry.Plugins.Mail.MailKit;

public class MailKitDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        if (container.Configuration.GetValue<bool>("Cofoundry:Plugins:MailKit:Disabled"))
        {
            return;
        }

        var overrideOptions = RegistrationOptions.Override();

        container
            .Register<IMailDispatchSession, MailKitMailDispatchSession>(overrideOptions)
            .Register<ISmtpClientConnectionConfiguration, SmtpClientConnectionConfiguration>()
            ;
    }
}
