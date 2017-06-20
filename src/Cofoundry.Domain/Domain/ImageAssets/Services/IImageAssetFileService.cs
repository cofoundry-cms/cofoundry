using Cofoundry.Domain.Data;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Abstraction over image asset file saving to give plugin libraries
    /// full control over the image saving process. This opens up cross platform
    /// image manipulation and additional optimization opportunities.
    /// </summary>
    public interface IImageAssetFileService
    {
        Task SaveAsync(IUploadedFile uploadedFile, ImageAsset imageAsset, string propertyName);
    }
}
