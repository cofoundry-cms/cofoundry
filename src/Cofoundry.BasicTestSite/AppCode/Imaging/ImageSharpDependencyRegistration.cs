using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.ImageSharp
{
    public class ImageResizerDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var overrideOptions = RegistrationOptions.Override();

            container
                .Register<IImageAssetFileService, ImageSharpImageAssetFileService>(overrideOptions)
                .Register<IResizedImageAssetFileService, ImageSharpResizedImageAssetFileService>(overrideOptions)
                ;
        }
    }
}
