using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    /// <remarks>
    /// This relates to how the old siteviewer device preview toggle worked, which was saved as 
    /// a global setting, which was a pretty bad way of doing it. This will/should eventually work differently in 
    /// the new (2015 refresh) site viewer.
    /// </remarks>
    public class UpdateVisualEditorSettingsCommandHandler
        : IAsyncCommandHandler<UpdateVisualEditorSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly SettingCommandHelper _settingCommandHelper;
        private readonly ISettingCache _settingCache;
        private readonly IPermissionValidationService _permissionValidationService;

        public UpdateVisualEditorSettingsCommandHandler(
            CofoundryDbContext dbContext,
            SettingCommandHelper settingCommandHelper,
            ISettingCache settingCache,
            IPermissionValidationService permissionValidationService
            )
        {
            _settingCommandHelper = settingCommandHelper;
            _dbContext = dbContext;
            _settingCache = settingCache;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdateVisualEditorSettingsCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);

            var allSettings = await _dbContext
                .Settings
                .ToListAsync();

            _settingCommandHelper.SetSettingProperty(command, c => c.VisualEditorDeviceView, allSettings, executionContext);

            await _dbContext.SaveChangesAsync();

            _settingCache.Clear();
        }

        #endregion
    }
}
