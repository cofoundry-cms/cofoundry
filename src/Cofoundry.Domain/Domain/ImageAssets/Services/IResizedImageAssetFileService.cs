using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IResizedImageAssetFileService
    {
        Stream Get(IImageAssetRenderable asset, IImageResizeSettings settings);
        void Clear(int imageAssetId);
    }
}
