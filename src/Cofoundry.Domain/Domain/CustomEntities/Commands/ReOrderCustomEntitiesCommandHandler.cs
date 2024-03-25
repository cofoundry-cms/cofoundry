using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Reorders a set of custom entities. The custom entity definition must implement 
/// IOrderableCustomEntityDefintion to be able to set ordering.
/// </summary>
public class ReOrderCustomEntitiesCommandHandler
    : ICommandHandler<ReOrderCustomEntitiesCommand>
    , IPermissionRestrictedCommandHandler<ReOrderCustomEntitiesCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public ReOrderCustomEntitiesCommandHandler(
        CofoundryDbContext dbContext,
        ICustomEntityStoredProcedures customEntityStoredProcedures,
        ICustomEntityCache customEntityCache,
        IMessageAggregator messageAggregator,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _customEntityStoredProcedures = customEntityStoredProcedures;
        _customEntityCache = customEntityCache;
        _messageAggregator = messageAggregator;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(ReOrderCustomEntitiesCommand command, IExecutionContext executionContext)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(command.CustomEntityDefinitionCode) as IOrderableCustomEntityDefinition;

        if (definition == null || definition.Ordering == CustomEntityOrdering.None)
        {
            throw new InvalidOperationException($"Cannot re-order a custom entity type with a definition that does not implement {nameof(IOrderableCustomEntityDefinition)} ({command.CustomEntityDefinitionCode})");
        }

        if (!definition.HasLocale && command.LocaleId.HasValue)
        {
            throw new ValidationException("Cannot order by locale because this custom entity type is not permitted to have a locale (" + definition.GetType().FullName + ")");
        }

        var affectedIds = await _customEntityStoredProcedures.ReOrderAsync(
            command.CustomEntityDefinitionCode,
            command.OrderedCustomEntityIds,
            command.LocaleId
            );

        await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, affectedIds));
    }

    private Task OnTransactionComplete(ReOrderCustomEntitiesCommand command, IReadOnlyCollection<int> affectedIds)
    {
        foreach (var affectedId in affectedIds)
        {
            _customEntityCache.Clear(command.CustomEntityDefinitionCode, affectedId);
        }

        var messages = affectedIds.Select(i => new CustomEntityOrderingUpdatedMessage()
        {
            CustomEntityDefinitionCode = command.CustomEntityDefinitionCode,
            CustomEntityId = i
        }).ToList();

        return _messageAggregator.PublishBatchAsync(messages);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(ReOrderCustomEntitiesCommand command)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(command.CustomEntityDefinitionCode);
        yield return new CustomEntityUpdatePermission(definition);
    }
}
