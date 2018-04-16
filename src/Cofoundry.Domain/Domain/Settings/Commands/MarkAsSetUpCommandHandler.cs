using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class MarkAsSetUpCommandHandler 
        : IAsyncCommandHandler<MarkAsSetUpCommand>
        , IIgnorePermissionCheckHandler
    {
        private const string SETTING_KEY = "IsSetup";

        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ISettingCache _settingCache;
        private readonly IPermissionValidationService _permissionValidationService;

        public MarkAsSetUpCommandHandler(
            CofoundryDbContext dbContext,
            SettingCommandHelper settingCommandHelper,
            ISettingCache settingCache,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _settingCache = settingCache;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(MarkAsSetUpCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsSuperAdminRole(executionContext.UserContext);

            var setting = await _dbContext
                .Settings
                .SingleOrDefaultAsync(s => s.SettingKey == SETTING_KEY);

            if (setting == null)
            {
                setting = new Setting();
                setting.SettingKey = SETTING_KEY;
                setting.CreateDate = executionContext.ExecutionDate;
                setting.UpdateDate = executionContext.ExecutionDate;
                _dbContext.Settings.Add(setting);
            }

            setting.SettingValue = "true";

            await _dbContext.SaveChangesAsync();
            _settingCache.Clear();
        }

        #endregion
    }
}
