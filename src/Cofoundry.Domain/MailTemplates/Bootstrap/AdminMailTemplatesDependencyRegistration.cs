using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.MailTemplates.AdminMailTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public class AdminMailTemplatesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<ICofoundryMailTemplateHelper, CofoundryMailTemplateHelper>()
                .Register<IUserMailTemplateBuilderFactory, UserMailTemplateBuilderFactory>()
                .Register<AdminMailTemplateUrlLibrary>()
                .RegisterAllGenericImplementations(typeof(IUserMailTemplateBuilder<>))
                ;
        }
    }
}
