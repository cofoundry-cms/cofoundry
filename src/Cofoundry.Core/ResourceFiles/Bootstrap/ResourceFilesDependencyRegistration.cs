using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.ResourceFiles.Internal;

namespace Cofoundry.Core.ResourceFiles.Registration
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
