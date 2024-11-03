using System.Diagnostics.CodeAnalysis;
using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// ImageSharp implementation of <see cref="IImageAssetFileService"/>.
/// </summary>
public class ImageSharpImageAssetFileService : IImageAssetFileService
{
    private const string ASSET_FILE_CONTAINER_NAME = "Images";

    private static readonly HashSet<string> _permittedImageFileExtensions = new(StringComparer.OrdinalIgnoreCase) {
        "jpg",
        "jpeg",
        "png",
        "gif",
        "webp"
    };

    private readonly CofoundryDbContext _dbContext;
    private readonly IFileStoreService _fileStoreService;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ImageAssetsSettings _imageAssetsSettings;
    private readonly ILogger<ImageSharpImageAssetFileService> _logger;

    public ImageSharpImageAssetFileService(
        CofoundryDbContext dbContext,
        IFileStoreService fileStoreService,
        ITransactionScopeManager transactionScopeManager,
        ImageAssetsSettings imageAssetsSettings,
        ILogger<ImageSharpImageAssetFileService> logger
        )
    {
        _dbContext = dbContext;
        _fileStoreService = fileStoreService;
        _transactionScopeManager = transactionScopeManager;
        _imageAssetsSettings = imageAssetsSettings;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(
        IFileSource fileToSave,
        ImageAsset imageAsset,
        string validationErrorPropertyName
        )
    {
        Image? imageFile = null;
        IImageFormat? imageFormat = null;

        using (var inputSteam = await fileToSave.OpenReadStreamAsync())
        {
            try
            {
                imageFile = Image.Load(inputSteam);
                imageFormat = imageFile.Metadata.DecodedImageFormat;
            }
            catch (InvalidImageContentException ex)
            {
                _logger.LogInformation(ex, "Image content error for file '{FileName}' with mime type {MimeType}.", fileToSave.FileName, fileToSave.MimeType);
                throw ValidationErrorException.CreateWithProperties("The file content is not valid.", validationErrorPropertyName);
            }
            catch (Exception ex) when (ex is NotSupportedException or ImageFormatException)
            {
                // We'll get an exception if the image file is invalid
                // so lets check to see if we can identify if it is an invalid file type and show that error
                // This might not always be the case since a file extension or mime type might not be supplied.
                var ext = Path.GetExtension(fileToSave.FileName);
                if ((!string.IsNullOrEmpty(ext) && !ImageAssetConstants.PermittedImageTypes.ContainsKey(ext))
                    || (!string.IsNullOrEmpty(fileToSave.MimeType) && !ImageAssetConstants.PermittedImageTypes.ContainsValue(fileToSave.MimeType)))
                {
                    throw ValidationErrorException.CreateWithProperties("The file is not a supported image type.", validationErrorPropertyName);
                }

                _logger.LogInformation(ex, "Image format error for file '{FileName}' with mime type {MimeType}.", fileToSave.FileName, fileToSave.MimeType);
                throw ValidationErrorException.CreateWithProperties("The file is not valid.", validationErrorPropertyName);
            }

            using (imageFile) // validate image file
            {
                ValidateImage(validationErrorPropertyName, imageFile, imageFormat);

                var requiredReEncoding = true;
                var fileExtension = "jpg";
                var foundExtension = _permittedImageFileExtensions
                    .FirstOrDefault(e => imageFormat.FileExtensions.Contains(e));

                if (foundExtension != null)
                {
                    fileExtension = foundExtension;
                    requiredReEncoding = false;
                }

                imageAsset.WidthInPixels = imageFile.Width;
                imageAsset.HeightInPixels = imageFile.Height;
                imageAsset.FileExtension = fileExtension;
                imageAsset.FileSizeInBytes = inputSteam.Length;

                using (var scope = _transactionScopeManager.Create(_dbContext))
                {
                    var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, imageAsset.FileExtension);

                    if (requiredReEncoding)
                    {
                        // Convert the image to jpg
                        using (var outputStream = new MemoryStream())
                        {
                            if (requiredReEncoding)
                            {
                                imageFile.Save(outputStream, new JpegEncoder());
                            }
                            else
                            {
                                imageFile.Save(outputStream, imageFormat);
                            }

                            await _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);

                            // recalculate size and save
                            imageAsset.FileSizeInBytes = outputStream.Length;
                        }
                    }
                    else
                    {
                        // Save the raw file directly
                        await _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, inputSteam);
                    }

                    await _dbContext.SaveChangesAsync();
                    await scope.CompleteAsync();
                };
            }
        }
    }

    private void ValidateImage(
        string propertyName,
        Image imageFile,
        [NotNull] IImageFormat? imageFormat
        )
    {
        if (imageFormat == null)
        {
            throw ValidationErrorException.CreateWithProperties("Unable to determine image type.", propertyName);
        }

        if (imageFile.Width > _imageAssetsSettings.MaxUploadWidth)
        {
            throw ValidationErrorException.CreateWithProperties($"Image exceeds the maximum permitted width of {_imageAssetsSettings.MaxUploadWidth} pixels.", propertyName);
        }
        if (imageFile.Height > _imageAssetsSettings.MaxUploadHeight)
        {
            throw ValidationErrorException.CreateWithProperties($"Image exceeds the maximum permitted height of {_imageAssetsSettings.MaxUploadHeight} pixels.", propertyName);
        }
    }
}
