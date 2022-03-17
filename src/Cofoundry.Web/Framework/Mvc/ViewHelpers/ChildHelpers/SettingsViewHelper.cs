namespace Cofoundry.Web;

public class SettingsViewHelper : ISettingsViewHelper
{
    private readonly IQueryExecutor _queryExecutor;

    public SettingsViewHelper(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public Task<TSettings> GetAsync<TSettings>() where TSettings : ICofoundrySettings
    {
        var query = new GetSettingsQuery<TSettings>();
        return _queryExecutor.ExecuteAsync(query);
    }
}