using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to validate an uploaded file by checking it's file extension
    /// and mime type. Use AssetFilesSettings to configure the validation
    /// behavior.
    /// </summary>
    public interface IAssetFileTypeValidator
    {
        /// <summary>
        /// Validates a file's extension and mime type, returning validation
        /// errors if they are invalid. Use AssetFilesSettings to configure 
        /// the validation behavior.
        /// </summary>
        /// <param name="fileNameOrFileExtension">
        /// The file name or file extension, with or without the leading dot. The
        /// check is case-insensitive.
        /// </param>
        /// <param name="mimeType">
        /// Mime type to check. The check is case-insensitive.
        /// </param>
        /// <param name="propertyName">
        /// The name of the model property being validated, used in the validation error.
        /// </param>
        IEnumerable<ValidationResult> Validate(string fileNameOrFileExtension, string mimeType, string propertyName);

        /// <summary>
        /// Validates a file's extension and mime type, throwing a validation
        /// exception if they are invalid. Use AssetFilesSettings to configure 
        /// the validation behavior.
        /// </summary>
        /// <param name="fileNameOrFileExtension">
        /// The file name or file extension, with or without the leading dot. The
        /// check is case-insensitive.
        /// </param>
        /// <param name="mimeType">
        /// Mime type to check. The check is case-insensitive.
        /// </param>
        /// <param name="propertyName">
        /// The name of the model property being validated, used in the validation exception.
        /// </param>
        void ValidateAndThrow(string fileNameOrFileExtension, string mimeType, string propertyName);
    }
}
