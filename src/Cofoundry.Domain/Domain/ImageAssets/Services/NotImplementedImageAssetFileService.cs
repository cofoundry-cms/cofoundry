using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Currently we're unable to offer a default cross-platform implementation out of the box.
/// </summary>
public class NotImplementedImageAssetFileService : IImageAssetFileService
{
    public Task SaveAsync(IFileSource uploadedFile, ImageAsset imageAsset, string propertyName)
    {
        throw new ImageAssetFileServiceNotImplementedException();
    }
}
