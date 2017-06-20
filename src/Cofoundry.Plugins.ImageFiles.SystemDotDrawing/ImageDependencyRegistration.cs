using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;

namespace Cofoundry.Plugins.ImageFiles.SystemDotDrawing
{
    public class ImageDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var options = RegistrationOptions.Override(RegistrationOverridePriority.Normal);

            container
                .RegisterType<IImageAssetFileService, SystemDrawingImageAssetFileService>(options)
                ;
        }
    }
}
