using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Creates a new draft version of a custom entity from the currently published version. If there
/// isn't a currently published version then an exception will be thrown. An exception is also 
/// thrown if there is already a draft version.
/// </summary>
public class AddCustomEntityDraftVersionCommandHandler
    : ICommandHandler<AddCustomEntityDraftVersionCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public AddCustomEntityDraftVersionCommandHandler(
        CofoundryDbContext dbContext,
        ICustomEntityCache customEntityCache,
        IMessageAggregator messageAggregator,
        IPermissionValidationService permissionValidationService,
        ICustomEntityStoredProcedures customEntityStoredProcedures,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _customEntityCache = customEntityCache;
        _messageAggregator = messageAggregator;
        _permissionValidationService = permissionValidationService;
        _customEntityStoredProcedures = customEntityStoredProcedures;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(AddCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
    {
        var definitionCode = await QueryVersionAndGetDefinitionCode(command).FirstOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(definitionCode, command.CustomEntityId);

        var user = _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(definitionCode, executionContext.UserContext);

        var newVersionId = await _customEntityStoredProcedures.AddDraftAsync(
            command.CustomEntityId,
            command.CopyFromCustomEntityVersionId,
            executionContext.ExecutionDate,
            user.UserId
            );

        command.OutputCustomEntityVersionId = newVersionId;

        await _transactionScopeManager.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(
            command,
            definitionCode,
            newVersionId)
            );
    }

    private Task OnTransactionComplete(AddCustomEntityDraftVersionCommand command, string definitionCode, int newVersionId)
    {
        _customEntityCache.Clear(definitionCode, command.CustomEntityId);

        return _messageAggregator.PublishAsync(new CustomEntityDraftVersionAddedMessage()
        {
            CustomEntityId = command.CustomEntityId,
            CustomEntityVersionId = newVersionId,
            CustomEntityDefinitionCode = definitionCode
        });
    }

    private IQueryable<string> QueryVersionAndGetDefinitionCode(AddCustomEntityDraftVersionCommand command)
    {
        // Query goes via the version to ensure one exists, even though we don't need to return it
        var dbQuery = _dbContext
            .CustomEntityVersions
            .AsNoTracking()
            .Where(v => v.CustomEntityId == command.CustomEntityId);

        if (command.CopyFromCustomEntityVersionId.HasValue)
        {
            dbQuery = dbQuery.Where(v => v.CustomEntityVersionId == command.CopyFromCustomEntityVersionId);
        }
        else
        {
            dbQuery = dbQuery
                .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .OrderByDescending(v => v.CreateDate);
        }

        return dbQuery
            .Select(v => v.CustomEntity.CustomEntityDefinitionCode);
    }
}
