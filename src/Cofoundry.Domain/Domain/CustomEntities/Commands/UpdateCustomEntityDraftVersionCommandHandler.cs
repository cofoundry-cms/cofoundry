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
using Cofoundry.Core.Validation;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the draft version of a custom entity. If a draft version
    /// does not exist then one is created first from the currently
    /// published version.
    /// </summary>
    public class UpdateCustomEntityDraftVersionCommandHandler 
        : ICommandHandler<UpdateCustomEntityDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<UpdateCustomEntityDraftVersionCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdateCustomEntityDraftVersionCommandHandler(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _messageAggregator = messageAggregator;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        public async Task ExecuteAsync(UpdateCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            var draft = await GetDraftVersionAsync(command);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                draft = await CreateDraftIfRequiredAsync(command, draft, executionContext);
                await ValidateTitleAsync(command, draft, definition, executionContext);
                UpdateDraft(command, draft);

                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionEntityDefinition.DefinitionCode,
                    draft.CustomEntityVersionId,
                    command.Model);

                await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);

                scope.QueueCompletionTask(() => OnUpdateDraftComplete(command, draft));

                if (command.Publish)
                {
                    await _commandExecutor.ExecuteAsync(new PublishCustomEntityCommand(draft.CustomEntityId, command.PublishDate), executionContext);
                }

                await scope.CompleteAsync();
            }
        }

        private Task OnUpdateDraftComplete(UpdateCustomEntityDraftVersionCommand command, CustomEntityVersion draft)
        {
            _customEntityCache.Clear(command.CustomEntityDefinitionCode, command.CustomEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityDraftVersionUpdatedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = command.CustomEntityDefinitionCode,
                CustomEntityVersionId = draft.CustomEntityVersionId
            });
        }

        private async Task ValidateTitleAsync(
            UpdateCustomEntityDraftVersionCommand command, 
            CustomEntityVersion dbVersion, 
            ICustomEntityDefinition definition,
            IExecutionContext executionContext
            )
        {
            if (!command.Publish || !definition.ForceUrlSlugUniqueness || SlugFormatter.ToSlug(dbVersion.Title) == dbVersion.CustomEntity.UrlSlug) return;

            var query = GetUniquenessQuery(command, definition, dbVersion);
            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                var message = string.Format("Cannot publish because the {1} '{0}' is not unique (symbols and spaces are ignored in the uniqueness check)",
                        command.Title,
                        definition.GetTerms().GetOrDefault(CustomizableCustomEntityTermKeys.Title, "title").ToLower());

                throw new UniqueConstraintViolationException(message, "Title", command.Title);
            }
        }

        private IsCustomEntityUrlSlugUniqueQuery GetUniquenessQuery(UpdateCustomEntityDraftVersionCommand command, ICustomEntityDefinition definition, CustomEntityVersion dbVersion)
        {
            var query = new IsCustomEntityUrlSlugUniqueQuery();
            query.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
            query.LocaleId = dbVersion.CustomEntity.LocaleId;
            query.UrlSlug = SlugFormatter.ToSlug(command.Title);
            query.CustomEntityId = command.CustomEntityId;

            return query;
        }

        private async Task<CustomEntityVersion> GetDraftVersionAsync(UpdateCustomEntityDraftVersionCommand command)
        {
            return await _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntity)
                .Where(p => p.CustomEntityId == command.CustomEntityId 
                    && p.WorkFlowStatusId == (int)WorkFlowStatus.Draft
                    && p.CustomEntity.CustomEntityDefinitionCode == command.CustomEntityDefinitionCode)
                .SingleOrDefaultAsync();
        }

        private async Task<CustomEntityVersion> CreateDraftIfRequiredAsync(
            UpdateCustomEntityDraftVersionCommand command, 
            CustomEntityVersion draft,
            IExecutionContext executionContext
            )
        {
            if (draft != null) return draft;

            var addDraftCommand = new AddCustomEntityDraftVersionCommand();
            addDraftCommand.CustomEntityId = command.CustomEntityId;
            await _commandExecutor.ExecuteAsync(addDraftCommand, executionContext);

            return await GetDraftVersionAsync(command);
        }

        private void UpdateDraft(UpdateCustomEntityDraftVersionCommand command, CustomEntityVersion draft)
        {
            EntityNotFoundException.ThrowIfNull(draft, "Draft:" + command.CustomEntityId);

            draft.Title = command.Title.Trim();
            draft.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.Model);
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCustomEntityDraftVersionCommand command)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            yield return new CustomEntityUpdatePermission(definition);

            if (command.Publish)
            {
                yield return new CustomEntityPublishPermission(definition);
            }
        }

        #endregion
    }
}
