﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Changes the order of a single custom entity. The custom entity 
/// definition must implement IOrderableCustomEntityDefintion to be 
/// able to set ordering.
/// </summary>
public class UpdateCustomEntityOrderingPositionCommandHandler
    : ICommandHandler<UpdateCustomEntityOrderingPositionCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;

    public UpdateCustomEntityOrderingPositionCommandHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
    }

    public async Task ExecuteAsync(UpdateCustomEntityOrderingPositionCommand command, IExecutionContext executionContext)
    {
        var customEntity = await _dbContext
            .CustomEntities
            .AsNoTracking()
            .FilterByCustomEntityId(command.CustomEntityId)
            .FirstOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(customEntity, command.CustomEntityId);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(customEntity.CustomEntityDefinitionCode, executionContext.UserContext);

        if (!command.Position.HasValue)
        {
            // The new position might be null (which means no ordering allocated)
            await SetOrderingNull(customEntity);
        }
        else
        {
            var orderedIds = await _dbContext
                .CustomEntities
                .Where(e => e.CustomEntityDefinitionCode == customEntity.CustomEntityDefinitionCode
                    && e.CustomEntityId != command.CustomEntityId
                    && e.Ordering.HasValue)
                .OrderBy(e => e.Ordering)
                .Select(e => e.CustomEntityId)
                .ToListAsync();

            var newPos = orderedIds.Count <= command.Position ? command.Position : null;

            if (!newPos.HasValue)
            {
                await SetOrderingNull(customEntity);
            }
            else
            {
                orderedIds.Insert(newPos.Value, command.CustomEntityId);

                var reorderCommand = new ReOrderCustomEntitiesCommand();
                reorderCommand.CustomEntityDefinitionCode = customEntity.CustomEntityDefinitionCode;
                reorderCommand.OrderedCustomEntityIds = orderedIds.ToArray();
            }
        }

    }

    private async Task SetOrderingNull(CustomEntity customEntity)
    {
        customEntity.Ordering = null;
        await _dbContext.SaveChangesAsync();
    }
}