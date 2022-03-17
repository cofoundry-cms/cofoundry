using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.Data;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Deletes any dependency references in the UnstructuredDataDependency table
/// relating to the specified entity.
/// </summary>
/// <remakrs>
/// Cofoundry entities do not use this because they handle the deletion via
/// triggers in the database, however this command can be used by anyone that
/// isn't able to manage database triggers for their dependent entities.
/// </remakrs>
public class DeleteUnstructuredDataDependenciesCommandHandler
    : ICommandHandler<DeleteUnstructuredDataDependenciesCommand>
    , IPermissionRestrictedCommandHandler<DeleteUnstructuredDataDependenciesCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
    private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;

    public DeleteUnstructuredDataDependenciesCommandHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository,
        IPermissionRepository permissionRepository,
        IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
        IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
        _permissionRepository = permissionRepository;
        _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
        _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
    }

    public async Task ExecuteAsync(DeleteUnstructuredDataDependenciesCommand command, IExecutionContext executionContext)
    {
        await _dependableEntityDeleteCommandValidator.ValidateAsync(command.RootEntityDefinitionCode, command.RootEntityId, executionContext);

        await _entityFrameworkSqlExecutor
            .ExecuteCommandAsync(_dbContext,
                "Cofoundry.UnstructuredDataDependency_Delete",
                new SqlParameter("EntityDefinitionCode", command.RootEntityDefinitionCode),
                new SqlParameter("EntityId", command.RootEntityId)
                );
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DeleteUnstructuredDataDependenciesCommand command)
    {
        var entityDefinition = _entityDefinitionRepository.GetByCode(command.RootEntityDefinitionCode);
        EntityNotFoundException.ThrowIfNull(entityDefinition, command.RootEntityDefinitionCode);

        var permission = _permissionRepository.GetByEntityAndPermissionType(entityDefinition, CommonPermissionTypes.Delete("Entity"));

        if (permission != null)
        {
            yield return permission;
        }
    }
}
