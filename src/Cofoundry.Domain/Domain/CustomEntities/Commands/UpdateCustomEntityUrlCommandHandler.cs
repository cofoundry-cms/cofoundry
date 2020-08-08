using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Validation;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the UrlSlug and locale of a custom entity which often forms
    /// the identity of the entity and can form part fo the url when used in
    /// custom entity pages. This is a specific action that can
    /// have specific side effects such as breaking page links outside
    /// of the CMS.
    /// </summary>
    public class UpdateCustomEntityUrlCommandHandler 
        : ICommandHandler<UpdateCustomEntityUrlCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdateCustomEntityUrlCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        public async Task ExecuteAsync(UpdateCustomEntityUrlCommand command, IExecutionContext executionContext)
        {
            var entity = await _dbContext
                .CustomEntities
                .Where(e => e.CustomEntityId == command.CustomEntityId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(entity, command.CustomEntityId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdateUrlPermission>(entity.CustomEntityDefinitionCode, executionContext.UserContext);

            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(entity.CustomEntityDefinitionCode);
            var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(definition, entity.CustomEntityDefinitionCode);

            await ValidateIsUniqueAsync(command, definition, executionContext);

            Map(command, entity, definition);

            await _dbContext.SaveChangesAsync();

            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(entity));
        }

        private Task OnTransactionComplete(CustomEntity entity)
        {
            _customEntityCache.Clear(entity.CustomEntityDefinitionCode, entity.CustomEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityUrlChangedMessage()
            {
                CustomEntityId = entity.CustomEntityId,
                CustomEntityDefinitionCode = entity.CustomEntityDefinitionCode,
                HasPublishedVersionChanged = entity.PublishStatusCode == PublishStatusCode.Published
            });
        }

        private async Task ValidateIsUniqueAsync(
            UpdateCustomEntityUrlCommand command, 
            CustomEntityDefinitionSummary definition,
            IExecutionContext executionContext
            )
        {
            if (!definition.ForceUrlSlugUniqueness) return;

            var query = new IsCustomEntityUrlSlugUniqueQuery();
            query.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
            query.CustomEntityId = command.CustomEntityId;
            query.LocaleId = command.LocaleId;
            query.UrlSlug = command.UrlSlug;

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);
            if (!isUnique)
            {
                var message = string.Format("A {0} already exists with the {2} '{1}'", 
                    definition.Name, 
                    command.UrlSlug, 
                    definition.Terms.GetOrDefault(CustomizableCustomEntityTermKeys.UrlSlug, "url slug").ToLower());
                throw new UniqueConstraintViolationException(message, "UrlSlug", command.UrlSlug);
            }
        }

        private void Map(UpdateCustomEntityUrlCommand command, CustomEntity entity, CustomEntityDefinitionSummary definition)
        {
            entity.UrlSlug = command.UrlSlug;
            entity.LocaleId = command.LocaleId;
        }
    }
}
