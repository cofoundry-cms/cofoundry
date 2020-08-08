using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Currently we're unable to offer a default cross-platform implementation out of the box.
    /// </summary>
    public class NotImplementedImageAssetFileService : IImageAssetFileService
    {
        public Task SaveAsync(IUploadedFile uploadedFile, ImageAsset imageAsset, string propertyName)
        {
            throw new ImageAssetFileServiceNotImplementedException();
        }
    }
}
