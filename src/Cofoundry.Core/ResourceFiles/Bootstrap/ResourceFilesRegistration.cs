using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.ResourceFiles
{
    public class ResourceFilesRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterAll<IAssemblyResourceRegistration>();
            container.RegisterAll<IEmbeddedResourceRouteRegistration>();
        }
    }
}
