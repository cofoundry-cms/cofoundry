using Cofoundry.Core.Data;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.Imaging.ImageSharp
{
    public class ImageSharpImageAssetFileService : IImageAssetFileService
    {
        private const string ASSET_FILE_CONTAINER_NAME = "Images";

        private static readonly HashSet<string> _permittedImageFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "jpg",
            "jpeg",
            "png",
            "gif"
        };

        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IFileStoreService _fileStoreService;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly ImageAssetsSettings _imageAssetsSettings;

        public ImageSharpImageAssetFileService(
            CofoundryDbContext dbContext,
            IFileStoreService fileStoreService,
            ITransactionScopeManager transactionScopeManager,
            ImageAssetsSettings imageAssetsSettings
            )
        {
            _dbContext = dbContext;
            _fileStoreService = fileStoreService;
            _transactionScopeManager = transactionScopeManager;
            _imageAssetsSettings = imageAssetsSettings;
        }

        #endregion

        public async Task SaveAsync(
            IUploadedFile uploadedFile,
            ImageAsset imageAsset,
            string propertyName
            )
        {
            Image imageFile = null;
            IImageFormat imageFormat = null;

            using (var inputSteam = await uploadedFile.OpenReadStreamAsync())
            {
                try
                {
                    imageFile = Image.Load(inputSteam, out imageFormat);
                }
                catch (ArgumentException ex)
                {
                    // We'll get an argument exception if the image file is invalid
                    // so lets check to see if we can identify if it is an invalid file type and show that error
                    // This might not always be the case since a file extension or mime type might not be supplied.
                    var ext = Path.GetExtension(uploadedFile.FileName);
                    if ((!string.IsNullOrEmpty(ext) && !ImageAssetConstants.PermittedImageTypes.ContainsKey(ext))
                        || (!string.IsNullOrEmpty(uploadedFile.MimeType) && !ImageAssetConstants.PermittedImageTypes.ContainsValue(uploadedFile.MimeType)))
                    {
                        throw new PropertyValidationException("The file is not a supported image type.", propertyName);
                    }

                    throw;
                }

                using (imageFile) // validate image file
                {
                    ValidateImage(propertyName, imageFile, imageFormat);

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
            IImageFormat imageFormat
            )
        {
            if (imageFormat == null)
            {
                throw new PropertyValidationException("Unable to determine image type.", propertyName);
            }

            if (imageFile.Width > _imageAssetsSettings.MaxUploadWidth)
            {
                throw new PropertyValidationException($"Image exceeds the maximum permitted width of {_imageAssetsSettings.MaxUploadWidth} pixels.", propertyName);
            }
            if (imageFile.Height > _imageAssetsSettings.MaxUploadHeight)
            {
                throw new PropertyValidationException($"Image exceeds the maximum permitted height of {_imageAssetsSettings.MaxUploadHeight} pixels.", propertyName);
            }
        }
    }
}
