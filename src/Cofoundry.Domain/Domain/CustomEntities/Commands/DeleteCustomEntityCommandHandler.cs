using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Deletes a custom entity and all associated versions permanently.
/// </summary>
public class DeleteCustomEntityCommandHandler
    : ICommandHandler<DeleteCustomEntityCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;

    public DeleteCustomEntityCommandHandler(
        CofoundryDbContext dbContext,
        ICustomEntityCache customEntityCache,
        IMessageAggregator messageAggregator,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeFactory,
        IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator,
        ICustomEntityStoredProcedures customEntityStoredProcedures
        )
    {
        _dbContext = dbContext;
        _customEntityCache = customEntityCache;
        _messageAggregator = messageAggregator;
        _permissionValidationService = permissionValidationService;
        _transactionScopeFactory = transactionScopeFactory;
        _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
        _customEntityStoredProcedures = customEntityStoredProcedures;
    }

    public async Task ExecuteAsync(DeleteCustomEntityCommand command, IExecutionContext executionContext)
    {
        var customEntity = await _dbContext
            .CustomEntities
            .SingleOrDefaultAsync(p => p.CustomEntityId == command.CustomEntityId);

        if (customEntity != null)
        {
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(customEntity.CustomEntityDefinitionCode, executionContext.UserContext);
            await _dependableEntityDeleteCommandValidator.ValidateAsync(customEntity.CustomEntityDefinitionCode, command.CustomEntityId, executionContext);

            _dbContext.CustomEntities.Remove(customEntity);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.CustomEntityId);

                scope.QueueCompletionTask(() => OnTransactionComplete(command, customEntity));
                await scope.CompleteAsync();
            }
        }
    }

    private Task OnTransactionComplete(DeleteCustomEntityCommand command, CustomEntity customEntity)
    {
        _customEntityCache.Clear(customEntity.CustomEntityDefinitionCode, command.CustomEntityId);

        return _messageAggregator.PublishAsync(new CustomEntityDeletedMessage()
        {
            CustomEntityId = command.CustomEntityId,
            CustomEntityDefinitionCode = customEntity.CustomEntityDefinitionCode
        });
    }
}
