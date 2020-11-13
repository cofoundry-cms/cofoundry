using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class AssetFileTypeValidatorTests
    {
        const string JPEG_FILE_EXTENSION = "jpeg";
        const string VALID_FILE_PATH = "/path/to/file.jpeg";
        const string CSV_FILE_EXTENSION = "csv";
        const string JPEG_MIME_TYPE = "image/jpeg";
        const string CSV_MIME_TYPE = "text/csv";
        const string VALIDATOR_PROP_NAME = "Test";

        private string[] FileTypeList = new string[] { JPEG_FILE_EXTENSION, "zip", CSV_FILE_EXTENSION };
        private string[] MimeTypeList = new string[] { JPEG_MIME_TYPE, "application/zip", CSV_MIME_TYPE };

        #region helpers

        private AssetFileTypeValidator CreateValidator(
            AssetFileTypeValidation fileExtensionValidation,
            AssetFileTypeValidation mimeTypeValidation
            )
        {
            var settings = new AssetFilesSettings()
            {
                FileExtensionValidation = fileExtensionValidation,
                FileExtensionValidationList = FileTypeList,
                MimeTypeValidation = mimeTypeValidation,
                MimeTypeValidationList = MimeTypeList
            };

            return new AssetFileTypeValidator(settings);
        }

        #endregion

        #region Validate

        [Fact]
        public void Validate_WhenDisabled_IsValid()
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.Disabled);
            var fileExtension = "rubbish";
            var mimeType = "rubbish";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Fact]
        public void Validate_WhenNullExtension_IsInvalid()
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseBlockList, AssetFileTypeValidation.Disabled);
            string fileExtension = null;
            var mimeType = JPEG_MIME_TYPE;

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, "Disabled", VALIDATOR_PROP_NAME));
        }

        [Fact]
        public void Validate_WhenNullMimeType_IsInvalid()
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseBlockList, AssetFileTypeValidation.UseAllowList);
            string fileExtension = JPEG_FILE_EXTENSION;
            string mimeType = null;

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, "Disabled", VALIDATOR_PROP_NAME));
        }

        [Theory]
        [InlineData(JPEG_FILE_EXTENSION)]
        [InlineData(CSV_FILE_EXTENSION)]
        [InlineData("." + CSV_FILE_EXTENSION)]
        [InlineData(VALID_FILE_PATH)]
        public void Validate_WhenAllowListAndValidExtension_IsValid(string fileExtension)
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseAllowList, AssetFileTypeValidation.Disabled);
            var mimeType = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(JPEG_FILE_EXTENSION)]
        [InlineData(CSV_FILE_EXTENSION)]
        [InlineData("." + CSV_FILE_EXTENSION)]
        [InlineData(VALID_FILE_PATH)]
        public void Validate_WhenAllowListAndValidUpperExtension_IsValid(string fileExtension)
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseAllowList, AssetFileTypeValidation.Disabled);
            var mimeType = "Disabled";

            validator.ValidateAndThrow(fileExtension.ToUpperInvariant(), mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension.ToUpperInvariant(), mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(JPEG_FILE_EXTENSION)]
        [InlineData(CSV_FILE_EXTENSION)]
        [InlineData("." + CSV_FILE_EXTENSION)]
        [InlineData(VALID_FILE_PATH)]
        public void Validate_WhenExtensionListHasDot_IsValid(string fileExtension)
        {
            var settings = new AssetFilesSettings()
            {
                FileExtensionValidation = AssetFileTypeValidation.UseAllowList,
                FileExtensionValidationList = FileTypeList.Select(f => "." + f).ToList(),
                MimeTypeValidation = AssetFileTypeValidation.Disabled,
                MimeTypeValidationList = MimeTypeList
            };

            var validator = new AssetFileTypeValidator(settings);
            var mimeType = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("rubbish")]
        [InlineData(".wibble")]
        [InlineData("/path/to/file.wibble")]
        public void Validate_WhenAllowListAndInvalidExtension_IsInvalid(string fileExtension)
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseAllowList, AssetFileTypeValidation.Disabled);
            var mimeType = "Disabled";

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME));
        }

        [Theory]
        [InlineData("pdf")]
        [InlineData(".wibble")]
        [InlineData("/path/to/file.wibble")]
        public void Validate_WhenDenyListAndValidExtension_IsValid(string fileExtension)
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseBlockList, AssetFileTypeValidation.Disabled);
            var mimeType = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(JPEG_FILE_EXTENSION)]
        [InlineData(CSV_FILE_EXTENSION)]
        [InlineData("." + CSV_FILE_EXTENSION)]
        [InlineData(VALID_FILE_PATH)]
        public void Validate_WhenDenyListAndInvalidExtension_IsInvalid(string fileExtension)
        {
            var validator = CreateValidator(AssetFileTypeValidation.UseBlockList, AssetFileTypeValidation.Disabled);
            var mimeType = "Disabled";

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME));
        }

        [Theory]
        [InlineData(JPEG_MIME_TYPE)]
        [InlineData(CSV_MIME_TYPE)]
        public void Validate_WhenAllowListAndValidMimeType_IsValid(string mimeType)
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.UseAllowList);
            var fileExtension = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(JPEG_MIME_TYPE)]
        [InlineData(CSV_MIME_TYPE)]
        public void Validate_WhenAllowListAndValidUpperMimeType_IsValid(string mimeType)
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.UseAllowList);
            var fileExtension = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType.ToUpperInvariant(), VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType.ToUpperInvariant(), VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("application/rubbish")]
        [InlineData("text/wibble")]
        public void Validate_WhenAllowListAndInvalidMimeType_IsInvalid(string mimeType)
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.UseAllowList);
            var fileExtension = "Disabled";

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME));
        }

        [Theory]
        [InlineData("application/rubbish")]
        [InlineData("text/wibble")]
        public void Validate_WhenDenyListAndValidMimeType_IsValid(string mimeType)
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.UseBlockList);
            var fileExtension = "Disabled";

            validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME);
            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(JPEG_MIME_TYPE)]
        [InlineData(CSV_MIME_TYPE)]
        public void Validate_WhenDenyListAndInvalidMimeType_IsInvalid(string mimeType)
        {
            var validator = CreateValidator(AssetFileTypeValidation.Disabled, AssetFileTypeValidation.UseBlockList);
            var fileExtension = "Disabled";

            var result = validator.Validate(fileExtension, mimeType, VALIDATOR_PROP_NAME);

            Assert.Single(result);
            Assert.Throws<ValidationErrorException>(() => validator.ValidateAndThrow(fileExtension, mimeType, VALIDATOR_PROP_NAME));
        }

        #endregion
    }
}
