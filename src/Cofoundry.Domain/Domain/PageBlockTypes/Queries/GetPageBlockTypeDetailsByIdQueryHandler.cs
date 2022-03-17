using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetPageBlockTypeDetailsByIdQueryHandler
    : IQueryHandler<GetPageBlockTypeDetailsByIdQuery, PageBlockTypeDetails>
    , IPermissionRestrictedQueryHandler<GetPageBlockTypeDetailsByIdQuery, PageBlockTypeDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageBlockTypeDetailsMapper _pageBlockTypeDetailsMapper;

    public GetPageBlockTypeDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageBlockTypeDetailsMapper pageBlockTypeDetailsMapper
        )
    {
        _queryExecutor = queryExecutor;
        _dbContext = dbContext;
        _pageBlockTypeDetailsMapper = pageBlockTypeDetailsMapper;
    }

    public async Task<PageBlockTypeDetails> ExecuteAsync(GetPageBlockTypeDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var blockTypeSummary = await GetPageBlockTypeById(query.PageBlockTypeId, executionContext);
        if (blockTypeSummary == null) return null;

        var result = _pageBlockTypeDetailsMapper.Map(blockTypeSummary);

        return result;
    }

    private async Task<PageBlockTypeSummary> GetPageBlockTypeById(int id, IExecutionContext executionContext)
    {
        var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

        var blockTypeTypeSummary = allBlockTypes
            .SingleOrDefault(t => t.PageBlockTypeId == id);

        return blockTypeTypeSummary;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageBlockTypeDetailsByIdQuery query)
    {
        yield return new PageReadPermission();
    }
}