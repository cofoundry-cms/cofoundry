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
    public class GetSiteViewerSettingsQueryHandler 
        : IQueryHandler<GetQuery<SiteViewerSettings>, SiteViewerSettings>
        , IAsyncQueryHandler<GetQuery<SiteViewerSettings>, SiteViewerSettings>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly SettingQueryHelper _getSettingQueryHelper;
        private readonly IInternalSettingsRepository _internalSettingsRepository;

        public GetSiteViewerSettingsQueryHandler(
            SettingQueryHelper getSettingQueryHelper,
            IInternalSettingsRepository internalSettingsRepository
            )
        {
            _getSettingQueryHelper = getSettingQueryHelper;
            _internalSettingsRepository = internalSettingsRepository;
        }

        #endregion

        #region execution

        public SiteViewerSettings Execute(GetQuery<SiteViewerSettings> query, IExecutionContext executionContext)
        {
            var allSettings = _internalSettingsRepository.GetAllSettings();
            return MapSettings(allSettings);
        }

        public async Task<SiteViewerSettings> ExecuteAsync(GetQuery<SiteViewerSettings> query, IExecutionContext executionContext)
        {
            var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
            return MapSettings(allSettings);
        }

        #endregion

        #region helpers

        private SiteViewerSettings MapSettings(Dictionary<string, string> allSettings)
        {
            var settings = new SiteViewerSettings();

            _getSettingQueryHelper.SetSettingProperty(settings, s => s.SiteViewerDeviceView, allSettings);

            return settings;
        }

        #endregion
    }
}
