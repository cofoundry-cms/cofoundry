using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Plugins.SiteMap;

public class DependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<ISiteMapBuilderFactory, SiteMapBuilderFactory>()
            .Register<ISiteMapRenderer, SiteMapRenderer>()

            .RegisterAll<ISiteMapResourceRegistration>()
            .RegisterAll<IAsyncSiteMapResourceRegistration>()
            ;
    }
}
