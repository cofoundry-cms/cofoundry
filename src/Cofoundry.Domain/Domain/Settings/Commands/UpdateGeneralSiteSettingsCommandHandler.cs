﻿using Cofoundry.Core.AutoUpdate;
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
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public UpdateGeneralSiteSettingsCommandHandler(
        CofoundryDbContext dbContext,
        SettingCommandHelper settingCommandHelper,
        ISettingCache settingCache,
        IAutoUpdateService autoUpdateService,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _settingCommandHelper = settingCommandHelper;
        _dbContext = dbContext;
        _settingCache = settingCache;
        _autoUpdateService = autoUpdateService;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(UpdateGeneralSiteSettingsCommand command, IExecutionContext executionContext)
    {
        var allSettings = await _dbContext
            .Settings
            .ToArrayAsync();

        _settingCommandHelper.SetSettingProperty(command, c => c.ApplicationName, allSettings, executionContext);

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _autoUpdateService.SetLockedAsync(!command.AllowAutomaticUpdates);

            await scope.CompleteAsync();
        }

        _transactionScopeFactory.QueueCompletionTask(_dbContext, _settingCache.Clear);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateGeneralSiteSettingsCommand command)
    {
        yield return new GeneralSettingsUpdatePermission();
    }
}