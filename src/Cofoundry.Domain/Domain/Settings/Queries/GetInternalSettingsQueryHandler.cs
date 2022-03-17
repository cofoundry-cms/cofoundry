namespace Cofoundry.Domain.Internal;

public class GetInternalSettingsQueryHandler
    : IQueryHandler<GetSettingsQuery<InternalSettings>, InternalSettings>
    , IIgnorePermissionCheckHandler
{
    private readonly SettingQueryHelper _getSettingQueryHelper;
    private readonly IInternalSettingsRepository _internalSettingsRepository;

    public GetInternalSettingsQueryHandler(
        SettingQueryHelper getSettingQueryHelper,
        IInternalSettingsRepository internalSettingsRepository
        )
    {
        _getSettingQueryHelper = getSettingQueryHelper;
        _internalSettingsRepository = internalSettingsRepository;
    }

    public async Task<InternalSettings> ExecuteAsync(GetSettingsQuery<InternalSettings> query, IExecutionContext executionContext)
    {
        var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
        return MapSettings(allSettings);
    }

    private InternalSettings MapSettings(Dictionary<string, string> allSettings)
    {
        var settings = new InternalSettings();

        _getSettingQueryHelper.SetSettingProperty(settings, s => s.IsSetup, allSettings);

        return settings;
    }
}