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
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public MarkAsSetUpCommandHandler(
        CofoundryDbContext dbContext,
        SettingCommandHelper settingCommandHelper,
        ISettingCache settingCache,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _settingCache = settingCache;
        _permissionValidationService = permissionValidationService;
        _transactionScopeFactory = transactionScopeFactory;
    }

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
        _transactionScopeFactory.QueueCompletionTask(_dbContext, _settingCache.Clear);
    }
}