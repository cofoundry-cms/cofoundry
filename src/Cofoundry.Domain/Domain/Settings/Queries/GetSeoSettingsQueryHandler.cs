using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetSeoSettingsQueryHandler 
        : IAsyncQueryHandler<GetQuery<SeoSettings>, SeoSettings>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

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

        #endregion

        #region execution

        public async Task<SeoSettings> ExecuteAsync(GetQuery<SeoSettings> query, IExecutionContext executionContext)
        {
            var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
            return MapSettings(allSettings);
        }

        private SeoSettings MapSettings(Dictionary<string, string> allSettings)
        {
            var settings = new SeoSettings();

            _getSettingQueryHelper.SetSettingProperty(settings, s => s.GoogleAnalyticsUAId, allSettings);
            _getSettingQueryHelper.SetSettingProperty(settings, s => s.HumansTxt, allSettings);
            _getSettingQueryHelper.SetSettingProperty(settings, s => s.RobotsTxt, allSettings);

            return settings;
        }

        #endregion
    }
}
