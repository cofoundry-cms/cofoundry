using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateDocumentAssetCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdateDocumentAssetCommand>, UpdateDocumentAssetCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdateDocumentAssetCommand>, UpdateDocumentAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;

    public GetUpdateDocumentAssetCommandByIdQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateDocumentAssetCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdateDocumentAssetCommand> query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .DocumentAssets
            .Include(a => a.DocumentAssetTags)
            .ThenInclude(a => a.Tag)
            .AsNoTracking()
            .FilterById(query.Id)
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;

        var result = new UpdateDocumentAssetCommand()
        {
            Description = dbResult.Description,
            DocumentAssetId = dbResult.DocumentAssetId,
            Title = dbResult.Title
        };

        result.Tags = dbResult
                .DocumentAssetTags
                .Select(t => t.Tag.TagText)
                .OrderBy(t => t)
                .ToArray();

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdateDocumentAssetCommand> query)
    {
        yield return new DocumentAssetReadPermission();
    }
}
