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
        /// <summary>
        /// Save an image asset file for either a new or updated image asset.
        /// </summary>
        /// <remarks>
        /// This will be run inside a transaction and if the image asset is new it will 
        /// have already been saved with temporary data in the WidthInPixels, HeightInPixels, 
        /// FileExtension and FileSizeInBytes properties in order to generate an ImageAssetId. 
        /// These properties should be updated in the method and you should call SaveAsync 
        /// on the dbContext before the method exits.
        /// </remarks>
        /// <param name="fileToSave">
        /// The file to save to disk. The file type and integrity should be checked within
        /// this method.
        /// </param>
        /// <param name="imageAsset">
        /// The image asset associated with the file. This could be a new or existing
        /// asset. The FileNameOnDisk should be used as the filename when saving the file
        /// and the the WidthInPixels, HeightInPixels, FileExtension and FileSizeInBytes 
        /// properties properties should be updated within this method.
        /// </param>
        /// <param name="validationErrorPropertyName">
        /// If a validation error is generated, this is the property name that should
        /// be used to associate the validation error with.
        /// </param>
        Task SaveAsync(IUploadedFile fileToSave, ImageAsset imageAsset, string validationErrorPropertyName);
    }
}
