using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Changes the order of a single custom entity. The custom entity 
    /// definition must implement IOrderableCustomEntityDefintion to be 
    /// able to set ordering.
    /// </summary>
    public class UpdateCustomEntityOrderingPositionCommandHandler
        : ICommandHandler<UpdateCustomEntityOrderingPositionCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly IPermissionValidationService _permissionValidationService;

        public UpdateCustomEntityOrderingPositionCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

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
}
