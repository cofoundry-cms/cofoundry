using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.ImageResizing.ImageSharp
{
    public class ImageResizerDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var overrideOptions = RegistrationOptions.Override();

            container
                .RegisterType<IImageAssetFileService, ImageSharpImageAssetFileService>(overrideOptions)
                .RegisterType<IResizedImageAssetFileService, ImageSharpResizedImageAssetFileService>(overrideOptions)
                ;
        }
    }
}
