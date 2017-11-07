using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class UnPublishCustomEntityCommandHandler 
        : IAsyncCommandHandler<UnPublishCustomEntityCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;

        public UnPublishCustomEntityCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory,
            ICustomEntityStoredProcedures customEntityStoredProcedures
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
            _customEntityStoredProcedures = customEntityStoredProcedures;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UnPublishCustomEntityCommand command, IExecutionContext executionContext)
        {
            var customEntity = await _dbContext
                .CustomEntities
                .Where(p => p.CustomEntityId == command.CustomEntityId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(customEntity, command.CustomEntityId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityPublishPermission>(customEntity.CustomEntityDefinitionCode, executionContext.UserContext);

            if (customEntity.PublishStatusCode == PublishStatusCode.Unpublished)
            {
                // No action
                return;
            }

            customEntity.PublishStatusCode = PublishStatusCode.Unpublished;

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.CustomEntityId);

                scope.Complete();
            }

            _customEntityCache.Clear(customEntity.CustomEntityDefinitionCode, command.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityUnPublishedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = customEntity.CustomEntityDefinitionCode
            });
        }

        #endregion
    }
}
