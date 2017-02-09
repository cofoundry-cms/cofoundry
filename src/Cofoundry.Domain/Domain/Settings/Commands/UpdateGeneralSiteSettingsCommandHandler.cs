using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain
{
    public class UpdateGeneralSiteSettingsCommandHandler 
        : IAsyncCommandHandler<UpdateGeneralSiteSettingsCommand>
        , IPermissionRestrictedCommandHandler<UpdateGeneralSiteSettingsCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly SettingCommandHelper _settingCommandHelper;
        private readonly ISettingCache _settingCache;
        private readonly IAutoUpdateService _autoUpdateService;

        public UpdateGeneralSiteSettingsCommandHandler(
            CofoundryDbContext dbContext,
            SettingCommandHelper settingCommandHelper,
            ISettingCache settingCache,
            IAutoUpdateService autoUpdateService
            )
        {
            _settingCommandHelper = settingCommandHelper;
            _dbContext = dbContext;
            _settingCache = settingCache;
            _autoUpdateService = autoUpdateService;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdateGeneralSiteSettingsCommand command, IExecutionContext executionContext)
        {
            var allSettings = await _dbContext
                .Settings
                .ToListAsync();

            _settingCommandHelper.SetSettingProperty(command, c => c.ApplicationName, allSettings, executionContext);

            await _dbContext.SaveChangesAsync();
            _autoUpdateService.SetLocked(!command.AllowAutomaticUpdates);

            _settingCache.Clear();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateGeneralSiteSettingsCommand command)
        {
            yield return new GeneralSettingsUpdatePermission();
        }

        #endregion
    }
}
