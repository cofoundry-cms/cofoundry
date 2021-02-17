using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.MailTemplates;
using Cofoundry.Domain.MailTemplates.AdminMailTemplates;
using Cofoundry.Domain.MailTemplates.DefaultMailTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Registration
{
    public class AdminMailTemplatesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<ICofoundryMailTemplateHelper, CofoundryMailTemplateHelper>()
                .Register<IUserMailTemplateBuilderFactory, UserMailTemplateBuilderFactory>()
                .Register<AdminMailTemplateUrlLibrary>()
                .Register<ICofoundryAdminMailTemplateBuilder, CofoundryAdminMailTemplateBuilder>()
                .RegisterGeneric(typeof(IDefaultMailTemplateBuilder<>), typeof(DefaultMailTemplateBuilder<>))
                .RegisterAllGenericImplementations(typeof(IUserMailTemplateBuilder<>))
                ;
        }
    }
}
