using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UpdateGeneralSiteSettingsCommandHandler
    : ICommandHandler<UpdateGeneralSiteSettingsCommand>
    , IPermissionRestrictedCommandHandler<UpdateGeneralSiteSettingsCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly SettingCommandHelper _settingCommandHelper;
    private readonly ISettingCache _settingCache;
    private readonly IAutoUpdateService _autoUpdateService;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public UpdateGeneralSiteSettingsCommandHandler(
        CofoundryDbContext dbContext,
        SettingCommandHelper settingCommandHelper,
        ISettingCache settingCache,
        IAutoUpdateService autoUpdateService,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _settingCommandHelper = settingCommandHelper;
        _dbContext = dbContext;
        _settingCache = settingCache;
        _autoUpdateService = autoUpdateService;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(UpdateGeneralSiteSettingsCommand command, IExecutionContext executionContext)
    {
        var allSettings = await _dbContext
            .Settings
            .ToArrayAsync();

        _settingCommandHelper.SetSettingProperty(command, c => c.ApplicationName, allSettings, executionContext);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _autoUpdateService.SetLockedAsync(!command.AllowAutomaticUpdates);

            await scope.CompleteAsync();
        }

        _transactionScopeManager.QueueCompletionTask(_dbContext, _settingCache.Clear);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateGeneralSiteSettingsCommand command)
    {
        yield return new GeneralSettingsUpdatePermission();
    }
}
