using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain
{
    public class GetGeneralSiteSettingsQueryHandler 
        : IAsyncQueryHandler<GetQuery<GeneralSiteSettings>, GeneralSiteSettings>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

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

        #endregion

        #region execution

        public async Task<GeneralSiteSettings> ExecuteAsync(GetQuery<GeneralSiteSettings> query, IExecutionContext executionContext)
        {
            var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
            return MapSettings(allSettings);
        }

        #endregion

        #region helpers

        private GeneralSiteSettings MapSettings(Dictionary<string, string> allSettings)
        {
            var settings = new GeneralSiteSettings();

            _getSettingQueryHelper.SetSettingProperty(settings, s => s.ApplicationName, allSettings);
            settings.AllowAutomaticUpdates = !_autoUpdateService.IsLocked();
            return settings;
        }

        #endregion
    }
}
