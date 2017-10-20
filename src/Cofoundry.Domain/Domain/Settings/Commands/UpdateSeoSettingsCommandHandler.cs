using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class UpdateSeoSettingsCommandHandler 
        : IAsyncCommandHandler<UpdateSeoSettingsCommand>
        , IPermissionRestrictedCommandHandler<UpdateSeoSettingsCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly SettingCommandHelper _settingCommandHelper;
        private readonly ISettingCache _settingCache;

        public UpdateSeoSettingsCommandHandler(
            CofoundryDbContext dbContext,
            SettingCommandHelper settingCommandHelper,
            ISettingCache settingCache
            )
        {
            _settingCommandHelper = settingCommandHelper;
            _dbContext = dbContext;
            _settingCache = settingCache;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdateSeoSettingsCommand command, IExecutionContext executionContext)
        {
            var allSettings = await _dbContext
                .Settings
                .ToListAsync();

            _settingCommandHelper.SetSettingProperty(command, c => c.GoogleAnalyticsUAId, allSettings, executionContext);
            _settingCommandHelper.SetSettingProperty(command, c => c.HumansTxt, allSettings, executionContext);
            _settingCommandHelper.SetSettingProperty(command, c => c.RobotsTxt, allSettings, executionContext);
            _settingCommandHelper.SetSettingProperty(command, c => c.BingWebmasterToolsApiKey, allSettings, executionContext);

            await _dbContext.SaveChangesAsync();

            _settingCache.Clear();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateSeoSettingsCommand command)
        {
            yield return new SeoSettingsUpdatePermission();
        }

        #endregion
    }
}
