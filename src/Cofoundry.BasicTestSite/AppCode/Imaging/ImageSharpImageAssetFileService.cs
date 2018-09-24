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

        public ImageSharpImageAssetFileService(
            CofoundryDbContext dbContext,
            IFileStoreService fileStoreService,
            ITransactionScopeManager transactionScopeManager
            )
        {
            _dbContext = dbContext;
            _fileStoreService = fileStoreService;
            _transactionScopeManager = transactionScopeManager;
        }

        #endregion

        public async Task SaveAsync(IUploadedFile uploadedFile, ImageAsset imageAsset, string propertyName)
        {
            Image<Rgba32> imageFile = null;
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
                    if (imageFormat == null) throw new PropertyValidationException("Unable to determine image type.", propertyName);

                    var requiredReEncoding = true;
                    var fileExtension = "jpg";
                    var foundExtension = _permittedImageFileExtensions
                        .FirstOrDefault(e => imageFormat.FileExtensions.Contains(e));

                    if (foundExtension != null)
                    {
                        fileExtension = foundExtension;
                        requiredReEncoding = false;
                    }

                    bool isNew = imageAsset.ImageAssetId < 1;

                    imageAsset.Width = imageFile.Width;
                    imageAsset.Height = imageFile.Height;
                    imageAsset.Extension = fileExtension;
                    imageAsset.FileSize = Convert.ToInt32(inputSteam.Length);

                    // Save at this point if it's a new image
                    if (isNew)
                    {
                        await _dbContext.SaveChangesAsync();
                    }

                    using (var scope = _transactionScopeManager.Create(_dbContext))
                    {
                        var fileName = Path.ChangeExtension(imageAsset.ImageAssetId.ToString(), imageAsset.Extension);

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
                                await CreateFileAsync(isNew, fileName, outputStream);
                                // recalculate size and save
                                imageAsset.FileSize = Convert.ToInt32(outputStream.Length);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            // Save the raw file directly
                            await CreateFileAsync(isNew, fileName, inputSteam);
                        }

                        await scope.CompleteAsync();
                    };
                }
            }
        }

        private Task CreateFileAsync(bool isNew, string fileName, Stream outputStream)
        {
            if (isNew)
            {
                return  _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);
            }
            else
            {
                return _fileStoreService.CreateOrReplaceAsync(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);
            }
        }
    }
}
