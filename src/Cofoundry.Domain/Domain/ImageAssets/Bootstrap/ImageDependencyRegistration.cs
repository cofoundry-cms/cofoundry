using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class ImageDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IImageAssetCache, ImageAssetCache>()
            .Register<IResizedImageAssetFileService, SimpleResizedImageAssetFileService>()
            .Register<IImageAssetRouteLibrary, ImageAssetRouteLibrary>()
            .Register<IImageAssetFileService, NotImplementedImageAssetFileService>()
            .Register<IImageAssetSummaryMapper, ImageAssetSummaryMapper>()
            .Register<IImageAssetDetailsMapper, ImageAssetDetailsMapper>()
            .Register<IImageAssetRenderDetailsMapper, ImageAssetRenderDetailsMapper>()
            .Register<IImageAssetFileMapper, ImageAssetFileMapper>()
            .Register<IImageResizeSettingsValidator, ImageResizeSettingsValidator>()
            ;
    }
}
