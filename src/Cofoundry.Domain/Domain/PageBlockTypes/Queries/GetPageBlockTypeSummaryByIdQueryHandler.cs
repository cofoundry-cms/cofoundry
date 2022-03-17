namespace Cofoundry.Domain.Internal;

public class GetPageBlockTypeSummaryByIdQueryHandler
    : IQueryHandler<GetPageBlockTypeSummaryByIdQuery, PageBlockTypeSummary>
    , IPermissionRestrictedQueryHandler<GetPageBlockTypeSummaryByIdQuery, PageBlockTypeSummary>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageBlockTypeSummaryByIdQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<PageBlockTypeSummary> ExecuteAsync(GetPageBlockTypeSummaryByIdQuery query, IExecutionContext executionContext)
    {
        var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);
        return allBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == query.PageBlockTypeId);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageBlockTypeSummaryByIdQuery query)
    {
        yield return new PageReadPermission();
    }
}