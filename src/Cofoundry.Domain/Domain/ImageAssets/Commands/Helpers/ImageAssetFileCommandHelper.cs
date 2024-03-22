using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Internal helper for handling the saving of assets in both
/// add and update image asset commands.
/// </summary>
public class ImageAssetFileCommandHelper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IImageAssetFileService _imageAssetFileService;
    private readonly IFileStoreService _fileStoreService;

    public ImageAssetFileCommandHelper(
        CofoundryDbContext dbContext,
        IImageAssetFileService imageAssetFileService,
        IFileStoreService fileStoreService
        )
    {
        _dbContext = dbContext;
        _imageAssetFileService = imageAssetFileService;
        _fileStoreService = fileStoreService;
    }

    public async Task SaveFileAsync(IFileSource fileToSave, ImageAsset imageAsset, string validationErrorPropertyName)
    {
        var extension = Path.GetExtension(fileToSave.FileName);

        // Svg files are handled differently to other image files because
        // they are just xml files with no special handling needed
        if (extension[1..] == ImageAssetConstants.SvgFileExtension)
        {
            var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, ImageAssetConstants.SvgFileExtension);
            using var svgStream = await fileToSave.OpenReadStreamAsync();

            await _fileStoreService.CreateAsync(ImageAssetConstants.FileContainerName, fileName, svgStream);
            imageAsset.FileExtension = ImageAssetConstants.SvgFileExtension;
            imageAsset.FileSizeInBytes = svgStream.Length;
            imageAsset.WidthInPixels = 0;
            imageAsset.HeightInPixels = 0;

            await _dbContext.SaveChangesAsync();

            return;
        }

        await _imageAssetFileService.SaveAsync(fileToSave, imageAsset, validationErrorPropertyName);
    }
}
