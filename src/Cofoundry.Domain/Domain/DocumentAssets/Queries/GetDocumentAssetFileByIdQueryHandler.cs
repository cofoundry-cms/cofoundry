using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetFileByIdQueryHandler
    : IQueryHandler<GetDocumentAssetFileByIdQuery, DocumentAssetFile>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetFileByIdQuery, DocumentAssetFile>
{
    private readonly IFileStoreService _fileStoreService;
    private readonly CofoundryDbContext _dbContext;

    public GetDocumentAssetFileByIdQueryHandler(
        IFileStoreService fileStoreService,
        CofoundryDbContext dbContext
        )
    {
        _fileStoreService = fileStoreService;
        _dbContext = dbContext;
    }

    public async Task<DocumentAssetFile> ExecuteAsync(GetDocumentAssetFileByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .DocumentAssets
            .Where(f => f.DocumentAssetId == query.DocumentAssetId)
            .Select(f => new
            {
                f.FileExtension,
                f.ContentType,
                f.FileName,
                f.FileUpdateDate,
                f.FileNameOnDisk,
                f.VerificationToken
            })
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;

        var result = new DocumentAssetFile()
        {
            DocumentAssetId = query.DocumentAssetId,
            ContentType = dbResult.ContentType,
            FileName = dbResult.FileName,
            FileNameOnDisk = dbResult.FileNameOnDisk,
            FileExtension = dbResult.FileExtension,
            FileUpdateDate = dbResult.FileUpdateDate,
            VerificationToken = dbResult.VerificationToken
        };

        result.FileStamp = AssetFileStampHelper.ToFileStamp(dbResult.FileUpdateDate);
        var fileName = Path.ChangeExtension(dbResult.FileNameOnDisk, dbResult.FileExtension);
        result.ContentStream = await _fileStoreService.GetAsync(DocumentAssetConstants.FileContainerName, fileName);

        if (result.ContentStream == null)
        {
            throw new FileNotFoundException("DocumentAsset file could not be found", fileName);
        }

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetFileByIdQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}