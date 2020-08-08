using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.ResourceFiles.DependencyRegistration
{
    public class ResourceFilesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IAssemblyResourceRegistration>()
                .RegisterAll<IEmbeddedResourceRouteRegistration>()
                .Register<IResourceFileProviderFactory, ResourceFileProviderFactory>()
                .Register<IEmbeddedFileProviderFactory, CofoundryEmbeddedFileProviderFactory>()
                ;
        }
    }
}
