using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class AdminMailTemplatesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterType<ICofoundryMailTemplatePageHelper, CofoundryMailTemplatePageHelper>();
        }
    }
}
