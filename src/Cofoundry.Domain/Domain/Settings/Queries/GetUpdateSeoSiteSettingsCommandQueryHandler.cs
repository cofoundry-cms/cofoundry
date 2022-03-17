namespace Cofoundry.Domain.Internal;

public class GetUpdateSeoSiteSettingsCommandQueryHandler
    : IQueryHandler<GetPatchableCommandQuery<UpdateSeoSettingsCommand>, UpdateSeoSettingsCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IQueryExecutor _queryExecutor;

    public GetUpdateSeoSiteSettingsCommandQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<UpdateSeoSettingsCommand> ExecuteAsync(GetPatchableCommandQuery<UpdateSeoSettingsCommand> query, IExecutionContext executionContext)
    {
        var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>(), executionContext);

        return new UpdateSeoSettingsCommand()
        {
            HumansTxt = settings.HumansTxt,
            RobotsTxt = settings.RobotsTxt
        };
    }
}
