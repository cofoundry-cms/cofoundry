using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetAllDocumentAssetFileTypesQueryHandler
    : IQueryHandler<GetAllDocumentAssetFileTypesQuery, IReadOnlyCollection<DocumentAssetFileType>>
    , IPermissionRestrictedQueryHandler<GetAllDocumentAssetFileTypesQuery, IReadOnlyCollection<DocumentAssetFileType>>
{
    private readonly CofoundryDbContext _dbContext;

    public GetAllDocumentAssetFileTypesQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<DocumentAssetFileType>> ExecuteAsync(GetAllDocumentAssetFileTypesQuery query, IExecutionContext executionContext)
    {
        var result = await _dbContext
            .DocumentAssets
            .AsNoTracking()
            .Select(a => a.FileExtension)
            .Distinct()
            .OrderBy(a => a)
            .Select(e => new DocumentAssetFileType() { FileExtension = e })
            .ToListAsync();

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllDocumentAssetFileTypesQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}