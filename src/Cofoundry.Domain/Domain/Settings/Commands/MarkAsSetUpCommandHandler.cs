using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class MarkAsSetUpCommandHandler
    : ICommandHandler<MarkAsSetUpCommand>
    , IIgnorePermissionCheckHandler
{
    private const string SETTING_KEY = "IsSetup";

    private readonly CofoundryDbContext _dbContext;
    private readonly ISettingCache _settingCache;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public MarkAsSetUpCommandHandler(
        CofoundryDbContext dbContext,
        ISettingCache settingCache,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _settingCache = settingCache;
        _permissionValidationService = permissionValidationService;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(MarkAsSetUpCommand command, IExecutionContext executionContext)
    {
        _permissionValidationService.EnforceIsSuperAdminRole(executionContext.UserContext);

        var setting = await _dbContext
            .Settings
            .SingleOrDefaultAsync(s => s.SettingKey == SETTING_KEY);

        if (setting == null)
        {
            setting = new Setting
            {
                SettingKey = SETTING_KEY,
                CreateDate = executionContext.ExecutionDate,
                UpdateDate = executionContext.ExecutionDate
            };
            _dbContext.Settings.Add(setting);
        }

        setting.SettingValue = "true";

        await _dbContext.SaveChangesAsync();
        _transactionScopeManager.QueueCompletionTask(_dbContext, _settingCache.Clear);
    }
}
