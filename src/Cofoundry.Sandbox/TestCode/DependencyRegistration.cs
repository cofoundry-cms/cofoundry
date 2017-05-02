using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Sandbox.TestCode
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterAll<IExampleClass>();
        }
    }
}
