using Cofoundry.Plugins.ErrorLogging.Data;

namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class GetErrorDetailsByIdQueryHandler
    : IQueryHandler<GetErrorDetailsByIdQuery, ErrorDetails?>
    , IPermissionRestrictedQueryHandler<GetErrorDetailsByIdQuery, ErrorDetails?>
{
    private readonly ErrorLoggingDbContext _dbContext;

    public GetErrorDetailsByIdQueryHandler(
        ErrorLoggingDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorDetails?> ExecuteAsync(GetErrorDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var error = await _dbContext
            .Errors
            .AsNoTracking()
            .Where(u => u.ErrorId == query.ErrorId)
            .Select(e => new ErrorDetails()
            {
                CreateDate = e.CreateDate,
                Data = e.Data,
                EmailSent = e.EmailSent,
                ErrorId = e.ErrorId,
                ExceptionType = e.ExceptionType,
                Form = e.Form,
                QueryString = e.QueryString,
                Session = e.QueryString,
                Source = e.Source,
                StackTrace = e.StackTrace,
                Target = e.Target,
                Url = e.Url,
                UserAgent = e.UserAgent
            })
            .SingleOrDefaultAsync();

        return error;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetErrorDetailsByIdQuery query)
    {
        yield return new ErrorLogReadPermission();
    }
}
