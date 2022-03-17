using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdatePageVersionBlockCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageVersionBlockCommand>, UpdatePageVersionBlockCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageVersionBlockCommand>, UpdatePageVersionBlockCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;

    public GetUpdatePageVersionBlockCommandByIdQueryHandler(
        CofoundryDbContext dbContext,
        IPageVersionBlockModelMapper pageVersionBlockModelMapper
        )
    {
        _dbContext = dbContext;
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
    }

    public async Task<UpdatePageVersionBlockCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageVersionBlockCommand> query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .Where(b => b.PageVersionBlockId == query.Id)
            .Select(b => new
            {
                PageBlock = b,
                BlockTypeFileName = b.PageBlockType.FileName
            })
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;

        var result = Map(dbResult.PageBlock, dbResult.BlockTypeFileName);
        return result;
    }

    private UpdatePageVersionBlockCommand Map(PageVersionBlock pageVersionBlock, string blockTypeFileName)
    {
        var result = new UpdatePageVersionBlockCommand()
        {
            PageBlockTypeId = pageVersionBlock.PageBlockTypeId,
            PageBlockTypeTemplateId = pageVersionBlock.PageBlockTypeTemplateId,
            PageVersionBlockId = pageVersionBlock.PageVersionBlockId
        };

        result.DataModel = _pageVersionBlockModelMapper.MapDataModel(blockTypeFileName, pageVersionBlock);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageVersionBlockCommand> query)
    {
        yield return new PageReadPermission();
    }
}
