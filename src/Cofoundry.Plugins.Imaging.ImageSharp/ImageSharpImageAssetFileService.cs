using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using ImageSharp;
using ImageSharp.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.ImageResizing.ImageSharp
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
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public ImageSharpImageAssetFileService(
            CofoundryDbContext dbContext,
            IFileStoreService fileStoreService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _fileStoreService = fileStoreService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        public async Task SaveAsync(IUploadedFile uploadedFile, ImageAsset imageAsset, string propertyName)
        {
            Image<Rgba32> imageFile = null;

            using (var inputSteam = await uploadedFile.OpenReadStreamAsync())
            {
                try
                {
                    imageFile = Image.Load(inputSteam);
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
                    var requiredReEncoding = true;
                    var fileExtension = "jpg";

                    if (_permittedImageFileExtensions.Contains(imageFile.CurrentImageFormat.Extension))
                    {
                        fileExtension = imageFile.CurrentImageFormat.Extension;
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

                    using (var scope = _transactionScopeFactory.Create(_dbContext))
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
                                    imageFile.Save(outputStream);
                                }
                                CreateFile(isNew, fileName, outputStream);
                                // recalculate size and save
                                imageAsset.FileSize = Convert.ToInt32(outputStream.Length);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            // Save the raw file directly
                            CreateFile(isNew, fileName, inputSteam);
                        }

                        scope.Complete();
                    };
                }
            }
        }

        private void CreateFile(bool isNew, string fileName, Stream outputStream)
        {
            if (isNew)
            {
                _fileStoreService.Create(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);
            }
            else
            {
                _fileStoreService.CreateOrReplace(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);
            }
        }
    }
}
