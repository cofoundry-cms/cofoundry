﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateCustomEntityDraftVersionCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdateCustomEntityDraftVersionCommand>, UpdateCustomEntityDraftVersionCommand?>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
    private readonly IPermissionValidationService _permissionValidationService;

    public GetUpdateCustomEntityDraftVersionCommandByIdQueryHandler(
        CofoundryDbContext dbContext,
        ICustomEntityDataModelMapper customEntityDataModelMapper,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _customEntityDataModelMapper = customEntityDataModelMapper;
        _permissionValidationService = permissionValidationService;
    }

    public async Task<UpdateCustomEntityDraftVersionCommand?> ExecuteAsync(GetPatchableCommandByIdQuery<UpdateCustomEntityDraftVersionCommand> query, IExecutionContext executionContext)
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

        command.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

        return command;
    }
}
