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
    public class GetVisualEditorSettingsQueryHandler
        : IAsyncQueryHandler<GetQuery<VisualEditorSettings>, VisualEditorSettings>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly SettingQueryHelper _getSettingQueryHelper;
        private readonly IInternalSettingsRepository _internalSettingsRepository;

        public GetVisualEditorSettingsQueryHandler(
            SettingQueryHelper getSettingQueryHelper,
            IInternalSettingsRepository internalSettingsRepository
            )
        {
            _getSettingQueryHelper = getSettingQueryHelper;
            _internalSettingsRepository = internalSettingsRepository;
        }

        #endregion

        #region execution

        public async Task<VisualEditorSettings> ExecuteAsync(GetQuery<VisualEditorSettings> query, IExecutionContext executionContext)
        {
            var allSettings = await _internalSettingsRepository.GetAllSettingsAsync();
            return MapSettings(allSettings);
        }

        #endregion

        #region helpers

        private VisualEditorSettings MapSettings(Dictionary<string, string> allSettings)
        {
            var settings = new VisualEditorSettings();

            _getSettingQueryHelper.SetSettingProperty(settings, s => s.VisualEditorDeviceView, allSettings);

            return settings;
        }

        #endregion
    }
}
