﻿using Cofoundry.Core.Data;
using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

public class DocumentAssetCommandHelper
{
    private static readonly string[] VALIDATION_MEMBER_FILE = ["File"];

    private readonly CofoundryDbContext _dbContext;
    private readonly IFileStoreService _fileStoreService;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IMimeTypeService _mimeTypeService;
    private readonly IAssetFileTypeValidator _assetFileTypeValidator;

    public DocumentAssetCommandHelper(
        CofoundryDbContext dbContext,
        IFileStoreService fileStoreService,
        ITransactionScopeManager transactionScopeFactory,
        IMimeTypeService mimeTypeService,
        IAssetFileTypeValidator assetFileTypeValidator
        )
    {
        _dbContext = dbContext;
        _fileStoreService = fileStoreService;
        _transactionScopeFactory = transactionScopeFactory;
        _mimeTypeService = mimeTypeService;
        _assetFileTypeValidator = assetFileTypeValidator;
    }

    public async Task SaveFile(IFileSource uploadedFile, DocumentAsset documentAsset)
    {
        documentAsset.ContentType = _mimeTypeService.MapFromFileName(uploadedFile.FileName, uploadedFile.MimeType);
        documentAsset.FileExtension = Path.GetExtension(uploadedFile.FileName).TrimStart('.');
        documentAsset.FileNameOnDisk = "file-not-saved";

        _assetFileTypeValidator.ValidateAndThrow(documentAsset.FileExtension, documentAsset.ContentType, "File");

        var fileStamp = AssetFileStampHelper.ToFileStamp(documentAsset.FileUpdateDate);

        using var inputSteam = await uploadedFile.OpenReadStreamAsync();
        bool isNew = documentAsset.DocumentAssetId < 1;

        documentAsset.FileSizeInBytes = inputSteam.Length;

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            // Save at this point if it's a new file
            if (isNew)
            {
                await _dbContext.SaveChangesAsync();
            }

            // update the filename
            documentAsset.FileNameOnDisk = $"{documentAsset.DocumentAssetId}-{fileStamp}";
            var fileName = Path.ChangeExtension(documentAsset.FileNameOnDisk, documentAsset.FileExtension);

            // Save the raw file directly
            await CreateFileAsync(isNew, fileName, inputSteam);

            // Update the filename
            await _dbContext.SaveChangesAsync();

            await scope.CompleteAsync();
        }
    }

    /// <summary>
    /// Some shared validation to prevent image asset types from being added to documents.
    /// </summary>
    public static IEnumerable<ValidationResult> Validate(IFileSource? file)
    {
        if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
        {
            // Validate extension & mime type

            var ext = Path.GetExtension(file.FileName)?.Trim();

            if (string.IsNullOrWhiteSpace(ext))
            {
                yield return new ValidationResult("The file you're uploading has no file extension.", VALIDATION_MEMBER_FILE);
            }
            else if (FilePathHelper.FileExtensionContainsInvalidChars(ext))
            {
                yield return new ValidationResult("The file you're uploading uses an extension containing invalid characters.", VALIDATION_MEMBER_FILE);
            }
            else if ((ImageAssetConstants.PermittedImageTypes.ContainsKey(ext))
            || (!string.IsNullOrEmpty(file.MimeType) && ImageAssetConstants.PermittedImageTypes.ContainsValue(file.MimeType)))
            {
                yield return new ValidationResult("Image files shoud be uploaded in the image assets section.", VALIDATION_MEMBER_FILE);
            }

            // Validate filename

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                yield return new ValidationResult("The file you're uploading has no file name.", VALIDATION_MEMBER_FILE);
            }
            else if (string.IsNullOrWhiteSpace(FilePathHelper.CleanFileName(file.FileName)))
            {
                yield return new ValidationResult("The file you're uploading has an invalid file name.", VALIDATION_MEMBER_FILE);
            }
        }
    }

    private Task CreateFileAsync(bool isNew, string fileName, Stream outputStream)
    {
        if (isNew)
        {
            return _fileStoreService.CreateAsync(DocumentAssetConstants.FileContainerName, fileName, outputStream);
        }
        else
        {
            return _fileStoreService.CreateOrReplaceAsync(DocumentAssetConstants.FileContainerName, fileName, outputStream);
        }
    }
}