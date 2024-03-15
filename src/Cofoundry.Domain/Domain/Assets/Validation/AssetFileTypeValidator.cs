namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IAssetFileTypeValidator"/>.
/// </summary>
public class AssetFileTypeValidator : IAssetFileTypeValidator
{
    private const string DEFAULT_FILE_TYPE_MESSAGE = "The type of file you're trying to add isn't allowed.";

    private readonly AssetFilesSettings _assetFileSettings;

    public AssetFileTypeValidator(
        AssetFilesSettings assetFileSettings
        )
    {
        _assetFileSettings = assetFileSettings;
    }

    /// <inheritdoc/>
    public virtual IEnumerable<ValidationResult> Validate(string? fileNameOrFileExtension, string? mimeType, string? propertyName)
    {
        propertyName ??= string.Empty;
        var formattedFileExtension = fileNameOrFileExtension;

        if (fileNameOrFileExtension != null && fileNameOrFileExtension.Contains('.'))
        {
            formattedFileExtension = Path.GetExtension(fileNameOrFileExtension ?? string.Empty).TrimStart('.');
        }

        if (!IsListValid(_assetFileSettings.MimeTypeValidation, _assetFileSettings.GetMimeTypeValidationListOrDefault(), mimeType)
            || !IsListValid(_assetFileSettings.FileExtensionValidation, GetFormattedFileExtensionList(), formattedFileExtension))
        {
            yield return new ValidationResult(DEFAULT_FILE_TYPE_MESSAGE, new string[] { propertyName });
        }
    }

    /// <inheritdoc/>
    public virtual void ValidateAndThrow(string? fileNameOrFileExtension, string? mimeType, string? propertyName)
    {
        var error = Validate(fileNameOrFileExtension, mimeType, propertyName).FirstOrDefault();

        if (error != null)
        {
            var message = error.ErrorMessage ?? DEFAULT_FILE_TYPE_MESSAGE;
            throw ValidationErrorException.CreateWithProperties(message, error.MemberNames?.ToArray());
        }
    }

    private IEnumerable<string> GetFormattedFileExtensionList()
    {
        return _assetFileSettings
            .GetFileExtensionValidationListOrDefault()
            .Select(l => l?.TrimStart('.'))
            .WhereNotNull();
    }

    private bool IsListValid(
        AssetFileTypeValidation assetFileTypeValidation,
        IEnumerable<string> list,
        string? itemToValidate
        )
    {
        if (assetFileTypeValidation == AssetFileTypeValidation.Disabled)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(itemToValidate))
        {
            return false;
        }

        var contains = list.Any(l => l != null && itemToValidate.Equals(l, StringComparison.OrdinalIgnoreCase));

        return assetFileTypeValidation switch
        {
            AssetFileTypeValidation.UseAllowList => contains,
            AssetFileTypeValidation.UseBlockList => !contains,
            _ => throw new Exception($"{nameof(AssetFileTypeValidation)} not recognised: {assetFileTypeValidation}"),
        };
    }
}
