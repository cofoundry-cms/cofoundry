using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Seaches roles based on simple filter criteria and returns a paged result. 
/// </summary>
public class SearchRolesQueryHandler
    : IQueryHandler<SearchRolesQuery, PagedQueryResult<RoleMicroSummary>>
    , IPermissionRestrictedQueryHandler<SearchRolesQuery, PagedQueryResult<RoleMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IRoleMicroSummaryMapper _roleMicroSummaryMapper;

    public SearchRolesQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IRoleMicroSummaryMapper roleMicroSummaryMapper
        )
    {
        _dbContext = dbContext;
        _roleMicroSummaryMapper = roleMicroSummaryMapper;
    }

    public async Task<PagedQueryResult<RoleMicroSummary>> ExecuteAsync(SearchRolesQuery query, IExecutionContext executionContext)
    {
        var dbPagedResult = await CreateQuery(query).ToPagedResultAsync(query);

        var mappedResults = dbPagedResult
            .Items
            .Select(_roleMicroSummaryMapper.Map);

        return dbPagedResult.ChangeType(mappedResults);
    }

    private IQueryable<Role> CreateQuery(SearchRolesQuery query)
    {
        var dbQuery = _dbContext
            .Roles
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.UserAreaCode))
        {
            dbQuery = dbQuery
                .Where(r => r.UserAreaCode == query.UserAreaCode);
        }

        if (query.ExcludeAnonymous)
        {
            dbQuery = dbQuery
                .Where(r => r.RoleCode != AnonymousRole.Code);
        }

        if (!string.IsNullOrEmpty(query.Text))
        {
            var text = query.Text.Trim();
            dbQuery = dbQuery.Where(r => r.Title.Contains(text))
                .OrderByDescending(r => r.Title.StartsWith(text))
                .ThenByDescending(r => r.Title);
        }
        else
        {
            dbQuery = dbQuery
                .OrderBy(r => r.UserArea.Name)
                .ThenBy(r => r.Title);
        }

        return dbQuery;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(SearchRolesQuery command)
    {
        yield return new RoleReadPermission();
    }
}