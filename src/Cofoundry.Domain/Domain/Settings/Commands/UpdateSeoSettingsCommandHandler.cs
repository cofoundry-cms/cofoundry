using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UpdateSeoSettingsCommandHandler
    : ICommandHandler<UpdateSeoSettingsCommand>
    , IPermissionRestrictedCommandHandler<UpdateSeoSettingsCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly SettingCommandHelper _settingCommandHelper;
    private readonly ISettingCache _settingCache;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public UpdateSeoSettingsCommandHandler(
        CofoundryDbContext dbContext,
        SettingCommandHelper settingCommandHelper,
        ISettingCache settingCache,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _settingCommandHelper = settingCommandHelper;
        _dbContext = dbContext;
        _settingCache = settingCache;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(UpdateSeoSettingsCommand command, IExecutionContext executionContext)
    {
        var allSettings = await _dbContext
            .Settings
            .ToListAsync();

        _settingCommandHelper.SetSettingProperty(command, c => c.HumansTxt, allSettings, executionContext);
        _settingCommandHelper.SetSettingProperty(command, c => c.RobotsTxt, allSettings, executionContext);

        await _dbContext.SaveChangesAsync();

        _transactionScopeFactory.QueueCompletionTask(_dbContext, _settingCache.Clear);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateSeoSettingsCommand command)
    {
        yield return new SeoSettingsUpdatePermission();
    }
}