using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.ModularMvc
{
    public class ModularMvcDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IViewLocationRegistration>()
                .RegisterAll<IBundleRegistration>()
                .RegisterAll<IRouteRegistration>()
                
                .RegisterInstance<VirtualPathProvider, AssemblyResourceProvider>()
                .RegisterInstance<IAssemblyResourcePhysicaFileRepository, AssemblyResourcePhysicaFileRepository>()
                
                .RegisterInstance<RazorViewEngine, AssemblyResourceViewEngine>()

                .RegisterType<IRouteInitializer, RouteInitializer>()
                .RegisterType<IBundleInitializer, BundleInitializer>()
                .RegisterType<IEmbeddedResourceRouteInitializer, EmbeddedResourceRouteInitializer>()
                .RegisterType<IAssemblyResourceViewEngineInitializer, AssemblyResourceViewEngineInitializer>()
                .RegisterType<IResourceLocator, WebsiteResourceLocator>()
                ;
        }
    }
}
