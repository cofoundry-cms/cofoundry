using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Creates a new custom entity, copying from an existing custom entity.
/// </summary>
public class DuplicateCustomEntityCommandHandler
    : ICommandHandler<DuplicateCustomEntityCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public DuplicateCustomEntityCommandHandler(
        ICommandExecutor commandExecutor,
        CofoundryDbContext dbContext,
        ICustomEntityStoredProcedures customEntityStoredProcedures,
        ITransactionScopeManager transactionScopeManager,
        ICustomEntityDataModelMapper customEntityDataModelMapper,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _commandExecutor = commandExecutor;
        _dbContext = dbContext;
        _customEntityStoredProcedures = customEntityStoredProcedures;
        _transactionScopeManager = transactionScopeManager;
        _customEntityDataModelMapper = customEntityDataModelMapper;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    public async Task ExecuteAsync(DuplicateCustomEntityCommand command, IExecutionContext executionContext)
    {
        var customEntityToDuplicate = await GetCustomEntityToDuplicateAsync(command);
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(customEntityToDuplicate.CustomEntity.CustomEntityDefinitionCode);
        var addCustomEntityCommand = MapCommand(command, customEntityToDuplicate);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            // Note: the underlying AddCustomEntityCommand will enforce permissions
            await _commandExecutor.ExecuteAsync(addCustomEntityCommand, executionContext);

            await _customEntityStoredProcedures.CopyBlocksToDraftAsync(
                addCustomEntityCommand.OutputCustomEntityId,
                customEntityToDuplicate.CustomEntityVersionId);

            if (definition.AutoPublish)
            {
                var publishCommand = new PublishCustomEntityCommand(addCustomEntityCommand.OutputCustomEntityId);
                await _commandExecutor.ExecuteAsync(publishCommand);
            }

            await scope.CompleteAsync();
        }

        // Set Ouput
        command.OutputCustomEntityId = addCustomEntityCommand.OutputCustomEntityId;
    }

    private async Task<CustomEntityVersion> GetCustomEntityToDuplicateAsync(DuplicateCustomEntityCommand command)
    {
        var result = await _dbContext
            .CustomEntityVersions
            .AsNoTracking()
            .Include(e => e.CustomEntity)
            .FilterActive()
            .FilterByCustomEntityId(command.CustomEntityToDuplicateId)
            .OrderByLatest()
            .FirstOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(result, command.CustomEntityToDuplicateId);

        return result;
    }

    private AddCustomEntityCommand MapCommand(
        DuplicateCustomEntityCommand command,
        CustomEntityVersion customEntityVersionToDuplicate
        )
    {
        var addCustomEntityCommand = new AddCustomEntityCommand();
        addCustomEntityCommand.Title = command.Title;
        addCustomEntityCommand.LocaleId = command.LocaleId;
        addCustomEntityCommand.UrlSlug = command.UrlSlug;
        addCustomEntityCommand.CustomEntityDefinitionCode = customEntityVersionToDuplicate.CustomEntity.CustomEntityDefinitionCode;
        addCustomEntityCommand.Model = _customEntityDataModelMapper.Map(addCustomEntityCommand.CustomEntityDefinitionCode, customEntityVersionToDuplicate.SerializedData);

        return addCustomEntityCommand;
    }
}
