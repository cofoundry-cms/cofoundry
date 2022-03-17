namespace Cofoundry.Domain.Internal;

public class GetPageDirectoryNodeByIdQueryHandler
    : IQueryHandler<GetPageDirectoryNodeByIdQuery, PageDirectoryNode>
    , IPermissionRestrictedQueryHandler<GetPageDirectoryNodeByIdQuery, PageDirectoryNode>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageDirectoryNodeByIdQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<PageDirectoryNode> ExecuteAsync(GetPageDirectoryNodeByIdQuery query, IExecutionContext executionContext)
    {
        var tree = await _queryExecutor.ExecuteAsync(new GetPageDirectoryTreeQuery(), executionContext);
        var result = tree
            .Flatten()
            .SingleOrDefault(n => n.PageDirectoryId == query.PageDirectoryId);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryNodeByIdQuery command)
    {
        yield return new PageDirectoryReadPermission();
    }
}
