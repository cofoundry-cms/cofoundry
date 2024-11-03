using System.Diagnostics.CodeAnalysis;
using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// SkiaSharp implementation of <see cref="IImageAssetFileService"/>.
/// </summary>
public class SkiaSharpImageAssetFileService : IImageAssetFileService
{
    private static readonly Dictionary<SKEncodedImageFormat, string> _permittedFormats = new()
    {
        { SKEncodedImageFormat.Gif, "gif" },
        { SKEncodedImageFormat.Jpeg, "jpg" },
        { SKEncodedImageFormat.Png, "png" },
        { SKEncodedImageFormat.Webp, "webp" }
    };

    private readonly CofoundryDbContext _dbContext;
    private readonly IFileStoreService _fileStoreService;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ImageAssetsSettings _imageAssetsSettings;
    private readonly SkiaSharpSettings _skiaSharpSettings;

    public SkiaSharpImageAssetFileService(
        CofoundryDbContext dbContext,
        IFileStoreService fileStoreService,
        ITransactionScopeManager transactionScopeManager,
        ImageAssetsSettings imageAssetsSettings,
        SkiaSharpSettings skiaSharpSettings
        )
    {
        _dbContext = dbContext;
        _fileStoreService = fileStoreService;
        _transactionScopeManager = transactionScopeManager;
        _imageAssetsSettings = imageAssetsSettings;
        _skiaSharpSettings = skiaSharpSettings;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(
        IFileSource fileToSave,
        ImageAsset imageAsset,
        string validationErrorPropertyName
        )
    {
        using var fileSource = ImageFileSource.Load(await fileToSave.OpenReadStreamAsync());

        ValidateCodec(fileSource.Codec, fileToSave, validationErrorPropertyName);
        ValidateImage(fileSource.Bitmap, validationErrorPropertyName);

        DetermineOutputFormat(fileSource.Codec, out var outputFormat, out var requiresReEncoding);

        imageAsset.WidthInPixels = fileSource.Bitmap.Width;
        imageAsset.HeightInPixels = fileSource.Bitmap.Height;
        imageAsset.FileExtension = _permittedFormats[outputFormat];
        imageAsset.FileSizeInBytes = fileSource.OriginalStream.Length;

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, imageAsset.FileExtension);

            if (requiresReEncoding)
            {
                // Convert the image (Quality setting seems to be ignored for pngs)
                using (var data = fileSource.Bitmap.Encode(outputFormat, _skiaSharpSettings.JpegQuality))
                using (var outputStream = data.AsStream())
                {
                    await _fileStoreService.CreateAsync(ImageAssetConstants.FileContainerName, fileName, outputStream);

                    // recalculate size and save
                    imageAsset.FileSizeInBytes = outputStream.Length;
                }
            }
            else
            {
                // Save the raw file directly
                fileSource.OriginalStream.Position = 0;
                await _fileStoreService.CreateAsync(ImageAssetConstants.FileContainerName, fileName, fileSource.OriginalStream);
            }

            await _dbContext.SaveChangesAsync();
            await scope.CompleteAsync();
        };
    }

    private void DetermineOutputFormat(SKCodec codec, out SKEncodedImageFormat imageFormat, out bool requiresReEncoding)
    {
        imageFormat = codec.EncodedFormat;
        requiresReEncoding = !_permittedFormats.ContainsKey(codec.EncodedFormat);
        if (!_permittedFormats.ContainsKey(codec.EncodedFormat))
        {
            imageFormat = SKEncodedImageFormat.Jpeg;
        }

        if (imageFormat == SKEncodedImageFormat.Gif)
        {
            if ((_skiaSharpSettings.GifResizeBehaviour == GifResizeBehaviour.Auto && codec.FrameCount < 2)
                || _skiaSharpSettings.GifResizeBehaviour == GifResizeBehaviour.ResizeAsAlternative)
            {
                imageFormat = SKEncodedImageFormat.Png;
                requiresReEncoding = true;
            }
        }
    }

    private void ValidateCodec(
        [NotNull] SKCodec? codec,
        IFileSource uploadedFile,
        string propertyName
        )
    {
        if (codec == null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.FileName);

            if ((!string.IsNullOrEmpty(fileExtension) && !ImageAssetConstants.PermittedImageTypes.ContainsKey(fileExtension))
                || (!string.IsNullOrEmpty(uploadedFile.MimeType) && !ImageAssetConstants.PermittedImageTypes.ContainsValue(uploadedFile.MimeType)))
            {
                throw ValidationErrorException.CreateWithProperties("The file is not a supported image type.", propertyName);
            }

            throw new Exception("SkiaSharp does not recognise the file as an image type.");
        }

        ValidateDimension(codec.Info.Width, _imageAssetsSettings.MaxUploadWidth, "width", propertyName);
        ValidateDimension(codec.Info.Height, _imageAssetsSettings.MaxUploadHeight, "height", propertyName);
    }

    private void ValidateImage(
        [NotNull] SKBitmap? bitmap,
        string propertyName
        )
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        // bitmap.IsEmpty
        ValidateDimension(bitmap.Width, _imageAssetsSettings.MaxUploadWidth, "width", propertyName);
        ValidateDimension(bitmap.Height, _imageAssetsSettings.MaxUploadHeight, "height", propertyName);
    }

    public void ValidateDimension(int amount, int maximum, string dimensionName, string propertyName)
    {
        if (amount > maximum)
        {
            throw ValidationErrorException.CreateWithProperties($"Image exceeds the {dimensionName} permitted width of {maximum} pixels.", propertyName);
        }
    }
}
