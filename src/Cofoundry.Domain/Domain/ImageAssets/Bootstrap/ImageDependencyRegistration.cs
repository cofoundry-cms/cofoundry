using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class ImageDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IImageAssetCache, ImageAssetCache>()
                .RegisterType<IResizedImageAssetFileService, SimpleResizedImageAssetFileService>()
                .RegisterType<ImageAssetCommandHelper>()
                .RegisterType<IImageAssetRouteLibrary, ImageAssetRouteLibrary>()
                .RegisterType<IImageAssetRepository, ImageAssetRepository>()
                ;
        }
    }
}
