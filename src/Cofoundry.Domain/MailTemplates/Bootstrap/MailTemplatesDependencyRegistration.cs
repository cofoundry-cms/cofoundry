using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain.MailTemplates.Registration
{
    public class MailTemplatesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<ICofoundryMailTemplateHelper, CofoundryMailTemplateHelper>()
                .RegisterAllGenericImplementations(typeof(IUserMailTemplateBuilder<>))
                .Register<IUserMailTemplateBuilderFactory, UserMailTemplateBuilderFactory>()
                .Register<IUserMailTemplateBuilderContextFactory, UserMailTemplateBuilderContextFactory>()
                .Register<IUserMailTemplateInitializer, UserMailTemplateInitializer>()
                .RegisterGeneric(typeof(IDefaultUserMailTemplateBuilder<>), typeof(DefaultUserMailTemplateBuilder<>))
                ;
        }
    }
}