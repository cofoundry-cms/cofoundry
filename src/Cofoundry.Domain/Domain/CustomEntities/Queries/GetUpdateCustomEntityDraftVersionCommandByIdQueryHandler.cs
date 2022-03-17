using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateCustomEntityDraftVersionCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdateCustomEntityDraftVersionCommand>, UpdateCustomEntityDraftVersionCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPermissionValidationService _permissionValidationService;

    public GetUpdateCustomEntityDraftVersionCommandByIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        _queryExecutor = queryExecutor;
        _permissionValidationService = permissionValidationService;
    }

    public async Task<UpdateCustomEntityDraftVersionCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdateCustomEntityDraftVersionCommand> query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .CustomEntityVersions
            .Include(v => v.CustomEntity)
            .AsNoTracking()
            .FilterActive()
            .FilterByCustomEntityId(query.Id)
            .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
            .SingleOrDefaultAsync();

        if (dbResult == null) return null;
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

        var command = new UpdateCustomEntityDraftVersionCommand()
        {
            CustomEntityDefinitionCode = dbResult.CustomEntity.CustomEntityDefinitionCode,
            CustomEntityId = dbResult.CustomEntityId,
            Title = dbResult.Title
        };

        var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(command.CustomEntityDefinitionCode);
        var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
        EntityNotFoundException.ThrowIfNull(definition, command.CustomEntityDefinitionCode);
        command.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbResult.SerializedData, definition.DataModelType);

        return command;
    }
}
