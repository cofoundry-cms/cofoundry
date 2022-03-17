using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateImageAssetCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdateImageAssetCommand>, UpdateImageAssetCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdateImageAssetCommand>, UpdateImageAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;

    public GetUpdateImageAssetCommandByIdQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateImageAssetCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdateImageAssetCommand> query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .ImageAssets
            .Include(a => a.ImageAssetTags)
            .ThenInclude(a => a.Tag)
            .AsNoTracking()
            .FilterById(query.Id)
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;

        var result = new UpdateImageAssetCommand()
        {
            ImageAssetId = dbResult.ImageAssetId,
            DefaultAnchorLocation = dbResult.DefaultAnchorLocation,
            Title = dbResult.Title
        };

        result.Tags = dbResult
                .ImageAssetTags
                .Select(t => t.Tag.TagText)
                .OrderBy(t => t)
                .ToArray();

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdateImageAssetCommand> query)
    {
        yield return new ImageAssetUpdatePermission();
    }
}
