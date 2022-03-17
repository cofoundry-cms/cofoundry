namespace Cofoundry.Domain.Internal;

public class GetUpdateGeneralSiteSettingsCommandQueryHandler
    : IQueryHandler<GetPatchableCommandQuery<UpdateGeneralSiteSettingsCommand>, UpdateGeneralSiteSettingsCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IQueryExecutor _queryExecutor;

    public GetUpdateGeneralSiteSettingsCommandQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<UpdateGeneralSiteSettingsCommand> ExecuteAsync(GetPatchableCommandQuery<UpdateGeneralSiteSettingsCommand> query, IExecutionContext executionContext)
    {
        var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<GeneralSiteSettings>(), executionContext);

        return new UpdateGeneralSiteSettingsCommand()
        {
            AllowAutomaticUpdates = settings.AllowAutomaticUpdates,
            ApplicationName = settings.ApplicationName
        };
    }
}
