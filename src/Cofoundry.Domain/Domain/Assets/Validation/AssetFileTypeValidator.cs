using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to validate an uploaded file by checking it's file extension
    /// and mime type. Use AssetFilesSettings to configure the validation
    /// behavior.
    /// </summary>
    public class AssetFileTypeValidator : IAssetFileTypeValidator
    {
        private readonly AssetFilesSettings _assetFileSettings;

        public AssetFileTypeValidator(
            AssetFilesSettings assetFileSettings
            )
        {
            _assetFileSettings = assetFileSettings;
        }

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
        public virtual IEnumerable<ValidationResult> Validate(string fileNameOrFileExtension, string mimeType, string propertyName)
        {
            propertyName = propertyName ?? string.Empty;
            var formattedFileExtension = fileNameOrFileExtension;

            if (fileNameOrFileExtension != null && fileNameOrFileExtension.Contains('.'))
            {
                formattedFileExtension = Path.GetExtension(fileNameOrFileExtension ?? string.Empty).TrimStart('.');
            }

            if (!IsListValid(_assetFileSettings.MimeTypeValidation, _assetFileSettings.GetMimeTypeValidationListOrDefault(), mimeType)
                || !IsListValid(_assetFileSettings.FileExtensionValidation, GetFormattedFileExtensionList(), formattedFileExtension))
            {
                yield return new ValidationResult("The type of file you're trying to add isn't allowed.", new string[] { propertyName });
            }
        }

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
        public virtual void ValidateAndThrow(string fileNameOrFileExtension, string mimeType, string propertyName)
        {
            var error = Validate(fileNameOrFileExtension, mimeType, propertyName).FirstOrDefault();
            
            if (error != null)
            {
                throw ValidationErrorException.CreateWithProperties(error.ErrorMessage, error.MemberNames?.ToArray());
            }
        }

        private IEnumerable<string> GetFormattedFileExtensionList()
        {
            return _assetFileSettings
                .GetFileExtensionValidationListOrDefault()
                .Select(l => l?.TrimStart('.'));
        }

        private bool IsListValid(
            AssetFileTypeValidation assetFileTypeValidation, 
            IEnumerable<string> list, 
            string itemToValidate
            )
        {
            if (assetFileTypeValidation == AssetFileTypeValidation.Disabled) return true;

            if (string.IsNullOrWhiteSpace(itemToValidate)) return false;

            var contains = list.Any(l => l != null && itemToValidate.Equals(l, StringComparison.OrdinalIgnoreCase));

            switch (assetFileTypeValidation)
            {
                case AssetFileTypeValidation.UseAllowList:
                    return contains;
                case AssetFileTypeValidation.UseBlockList:
                    return !contains;
                default:
                    throw new Exception($"{nameof(AssetFileTypeValidation)} not recognised: {assetFileTypeValidation}");
            }
        }
    }
}
