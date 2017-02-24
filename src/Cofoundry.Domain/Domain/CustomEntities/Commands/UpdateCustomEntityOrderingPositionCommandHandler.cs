using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using System.Data.SqlClient;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Set the position of the custopm entity in the order
    /// </summary>
    public class UpdateCustomEntityOrderingPositionCommandHandler
        : IAsyncCommandHandler<UpdateCustomEntityOrderingPositionCommand>
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

        #region execution

        public async Task ExecuteAsync(UpdateCustomEntityOrderingPositionCommand command, IExecutionContext executionContext)
        {
            var customEntity = await _dbContext
                .CustomEntities
                .AsNoTracking()
                .FilterById(command.CustomEntityId)
                .FirstOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(customEntity, command.CustomEntityId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(customEntity.CustomEntityDefinitionCode);

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

        #endregion
    }
}
