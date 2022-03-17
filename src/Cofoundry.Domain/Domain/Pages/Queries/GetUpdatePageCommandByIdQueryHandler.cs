using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdatePageCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageCommand>, UpdatePageCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageCommand>, UpdatePageCommand>
{
    private readonly CofoundryDbContext _dbContext;

    public GetUpdatePageCommandByIdQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<UpdatePageCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageCommand> query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.PageTags)
            .ThenInclude(t => t.Tag)
            .FilterActive()
            .FilterById(query.Id)
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;

        var command = new UpdatePageCommand()
        {
            PageId = dbResult.PageId,
            Tags = dbResult
                .PageTags
                .Select(t => t.Tag.TagText)
                .OrderBy(t => t)
                .ToArray()
        };

        return command;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageCommand> query)
    {
        yield return new PageReadPermission();
    }
}
