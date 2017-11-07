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

namespace Cofoundry.Domain
{
    public class UpdateCustomEntityUrlCommandHandler 
        : IAsyncCommandHandler<UpdateCustomEntityUrlCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;

        public UpdateCustomEntityUrlCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdateCustomEntityUrlCommand command, IExecutionContext executionContext)
        {
            var entity = await _dbContext
                .CustomEntities
                .Where(e => e.CustomEntityId == command.CustomEntityId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(entity, command.CustomEntityId);
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityUpdateUrlPermission>(entity.CustomEntityDefinitionCode);

            var definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(entity.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, entity.CustomEntityDefinitionCode);

            await ValidateIsUnique(command, definition);

            Map(command, entity, definition);

            await _dbContext.SaveChangesAsync();
            _customEntityCache.Clear(entity.CustomEntityDefinitionCode, command.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityUrlChangedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = entity.CustomEntityDefinitionCode,
                HasPublishedVersionChanged = entity.PublishStatusCode == PublishStatusCode.Published
            });
        }
        
        private async Task ValidateIsUnique(UpdateCustomEntityUrlCommand command, CustomEntityDefinitionSummary definition)
        {
            if (!definition.ForceUrlSlugUniqueness) return;

            var query = new IsCustomEntityPathUniqueQuery();
            query.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
            query.CustomEntityId = command.CustomEntityId;
            query.LocaleId = command.LocaleId;
            query.UrlSlug = command.UrlSlug;

            var isUnique = await _queryExecutor.ExecuteAsync(query);
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

        #endregion
    }
}
