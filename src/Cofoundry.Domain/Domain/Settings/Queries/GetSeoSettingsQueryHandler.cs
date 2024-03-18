﻿namespace Cofoundry.Domain.Internal;

public class GetSeoSettingsQueryHandler
    : IQueryHandler<GetSettingsQuery<SeoSettings>, SeoSettings>
    , IIgnorePermissionCheckHandler
{
    private readonly SettingQueryHelper _getSettingQueryHelper;
    private readonly IInternalSettingsRepository _internalSettingsRepository;

    public GetSeoSettingsQueryHandler(
        SettingQueryHelper getSettingQueryHelper,
        IInternalSettingsRepository internalSettingsRepository
        )
    {
        _getSettingQueryHelper = getSettingQueryHelper;
        _internalSettingsRepository = internalSettingsRepository;
    }

    public async Task<SeoSettings> ExecuteAsync(GetSettingsQuery<SeoSettings> query, IExecutionContext executionContext)
    {
        var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
        return MapSettings(allSettings);
    }

    private SeoSettings MapSettings(IReadOnlyDictionary<string, string> allSettings)
    {
        var settings = new SeoSettings();

        _getSettingQueryHelper.SetSettingProperty(settings, s => s.HumansTxt, allSettings);
        _getSettingQueryHelper.SetSettingProperty(settings, s => s.RobotsTxt, allSettings);

        return settings;
    }
}