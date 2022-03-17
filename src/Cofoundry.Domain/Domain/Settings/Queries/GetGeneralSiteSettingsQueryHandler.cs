using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Internal;

public class GetGeneralSiteSettingsQueryHandler
    : IQueryHandler<GetSettingsQuery<GeneralSiteSettings>, GeneralSiteSettings>
    , IIgnorePermissionCheckHandler
{
    private readonly SettingQueryHelper _getSettingQueryHelper;
    private readonly IAutoUpdateService _autoUpdateService;
    private readonly IInternalSettingsRepository _internalSettingsRepository;

    public GetGeneralSiteSettingsQueryHandler(
        SettingQueryHelper getSettingQueryHelper,
        IAutoUpdateService autoUpdateService,
        IInternalSettingsRepository internalSettingsRepository
        )
    {
        _getSettingQueryHelper = getSettingQueryHelper;
        _autoUpdateService = autoUpdateService;
        _internalSettingsRepository = internalSettingsRepository;
    }

    public async Task<GeneralSiteSettings> ExecuteAsync(GetSettingsQuery<GeneralSiteSettings> query, IExecutionContext executionContext)
    {
        var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
        return await MapSettingsAsync(allSettings);
    }

    private async Task<GeneralSiteSettings> MapSettingsAsync(Dictionary<string, string> allSettings)
    {
        var settings = new GeneralSiteSettings();

        _getSettingQueryHelper.SetSettingProperty(settings, s => s.ApplicationName, allSettings);

        var isLocked = await _autoUpdateService.IsLockedAsync();
        settings.AllowAutomaticUpdates = !isLocked;

        return settings;
    }
}