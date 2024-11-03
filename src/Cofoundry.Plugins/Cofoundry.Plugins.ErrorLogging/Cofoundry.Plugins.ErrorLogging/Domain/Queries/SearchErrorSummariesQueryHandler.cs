using Cofoundry.Domain.Data;
using Cofoundry.Plugins.ErrorLogging.Data;

namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class SearchErrorSummariesQueryHandler
    : IQueryHandler<SearchErrorSummariesQuery, PagedQueryResult<ErrorSummary>>
    , IPermissionRestrictedQueryHandler<SearchErrorSummariesQuery, PagedQueryResult<ErrorSummary>>
{
    private readonly ErrorLoggingDbContext _dbContext;

    public SearchErrorSummariesQueryHandler(
        ErrorLoggingDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<PagedQueryResult<ErrorSummary>> ExecuteAsync(SearchErrorSummariesQuery query, IExecutionContext executionContext)
    {
        var result = await CreateQuery(query).ToPagedResultAsync(query);

        return result;
    }

    private IQueryable<ErrorSummary> CreateQuery(SearchErrorSummariesQuery query)
    {
        var dbQuery = _dbContext
            .Errors
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Text))
        {
            dbQuery = dbQuery.Where(u =>
                u.Url!.Contains(query.Text)
                || u.UserAgent!.Contains(query.Text)
                || u.ExceptionType.Contains(query.Text)
                );
        }

        return dbQuery
            .OrderByDescending(u => u.CreateDate)
            .Select(e => new ErrorSummary()
            {
                CreateDate = e.CreateDate,
                ErrorId = e.ErrorId,
                ExceptionType = e.ExceptionType,
                Url = e.Url,
                UserAgent = e.UserAgent
            });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(SearchErrorSummariesQuery query)
    {
        yield return new ErrorLogReadPermission();
    }
}
