using Cofoundry.Core;
using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Configuration settings covering all assets type
    /// files, i.e. document and image asset files.
    /// </summary>
    public class AssetFilesSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Indicates the type of validation to perform against a file
        /// extension and is used in combination with the values in the 
        /// FileExtensionValidationList setting. The default is UseBlocklist,
        /// and other options are UseAllowlist or Disable.
        /// </summary>
        public AssetFileTypeValidation FileExtensionValidation { get; set; } = AssetFileTypeValidation.UseBlockList;

        /// <summary>
        /// Indicates the type of validation to perform against a mime
        /// type and is used in combination with the values in the 
        /// MimeTypeValidationList setting. The default is UseBlocklist,
        /// and other options are UseAllowlist or Disable.
        /// </summary>
        public AssetFileTypeValidation MimeTypeValidation { get; set; } = AssetFileTypeValidation.UseBlockList;

        /// <summary>
        /// The list of file extensions to use when validating an uploaded
        /// file by it's file extension. By default this is a list of
        /// potentially harmful file extensions and is treated as a blocklist, 
        /// but the FileExtensionValidation setting can be used to change this 
        /// behavior to interpret it as a allowlist, or disabled this validation 
        /// entirely.
        /// </summary>
        public ICollection<string> FileExtensionValidationList { get; set; }

        /// <summary>
        /// The list of mime types to use when validating an uploaded
        /// file by it's mime type. By default this is a list of potentially 
        /// harmful mime types and is treated as a blocklist, but the 
        /// MimeTypeValidation setting can be used to change this behavior 
        /// to interpret it as an allowlist, or disable this validation entirely.
        /// </summary>
        public ICollection<string> MimeTypeValidationList { get; set; }

        /// <summary>
        /// Returns the FileExtensionValidationList collection if
        /// it is not empty, otherwise returning the default values
        /// from DangerousFileConstants.DangerousFileExtensions.
        /// </summary>
        /// <remarks>
        /// We use this rather than setting defaults to the property
        /// because the asp.net core framework doesn't overwrite a collection
        /// initialized with a default value, instead it appends the 
        /// configuration from appsettings to it.
        /// </remarks>
        public ICollection<string> GetFileExtensionValidationListOrDefault()
        {
            return EnumerableHelper.IsNullOrEmpty(FileExtensionValidationList) ? DangerousFileConstants.DangerousFileExtensions : FileExtensionValidationList;
        }

        /// <summary>
        /// Returns the MimeTypeValidationList collection if
        /// it is not empty, otherwise returning the default values
        /// from DangerousFileConstants.DangerousMimeTypes.
        /// </summary>
        /// <remarks>
        /// We use this rather than setting defaults to the property
        /// because the asp.net core framework doesn't overwrite a collection
        /// initialized with a default value, instead it appends the 
        /// configuration from appsettings to it.
        /// </remarks>
        public ICollection<string> GetMimeTypeValidationListOrDefault()
        {
            return EnumerableHelper.IsNullOrEmpty(MimeTypeValidationList) ? DangerousFileConstants.DangerousMimeTypes : MimeTypeValidationList;
        }
    }
}
