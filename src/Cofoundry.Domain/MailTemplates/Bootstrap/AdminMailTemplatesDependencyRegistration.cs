using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Registration
{
    public class AdminMailTemplatesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.Register<ICofoundryMailTemplateHelper, CofoundryMailTemplateHelper>();
        }
    }
}
