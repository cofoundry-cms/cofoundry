using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class PageDirectoryDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPageDirectoryCache, PageDirectoryCache>()
                .Register<IPageDirectoryRepository, PageDirectoryRepository>()
                .Register<IPageDirectoryRouteMapper, PageDirectoryRouteMapper>()
                .Register<IPageDirectoryTreeMapper, PageDirectoryTreeMapper>()
                ;
        }
    }
}
