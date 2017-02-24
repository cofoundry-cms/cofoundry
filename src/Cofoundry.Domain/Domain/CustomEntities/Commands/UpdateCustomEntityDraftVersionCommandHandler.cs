using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class UpdateCustomEntityDraftVersionCommandHandler 
        : IAsyncCommandHandler<UpdateCustomEntityDraftVersionCommand>
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
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UpdateCustomEntityDraftVersionCommandHandler(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            ITransactionScopeFactory transactionScopeFactory
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

        #region execute

        public async Task ExecuteAsync(UpdateCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            var draft = await GetDraftVersionAsync(command);


            using (var scope = _transactionScopeFactory.Create())
            {
                draft = await CreateDraftIfRequiredAsync(command, draft);
                await ValidateTitle(command, draft, definition);
                UpdateDraft(command, draft);

                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionEntityDefinition.DefinitionCode,
                    draft.CustomEntityVersionId,
                    command.Model);

                await _commandExecutor.ExecuteAsync(dependencyCommand);

                scope.Complete();
            }

            _customEntityCache.Clear(command.CustomEntityDefinitionCode, command.CustomEntityId);
            await _messageAggregator.PublishAsync(new CustomEntityDraftVersionUpdatedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = command.CustomEntityDefinitionCode,
                CustomEntityVersionId = draft.CustomEntityVersionId
            });

            if (command.Publish)
            {
                using (var scope = _transactionScopeFactory.Create())
                {
                    await _commandExecutor.ExecuteAsync(new PublishCustomEntityCommand(draft.CustomEntityId));
                    scope.Complete();
                }
            }
        }

        #endregion

        #region helpers

        private async Task ValidateTitle(UpdateCustomEntityDraftVersionCommand command, CustomEntityVersion dbVersion, ICustomEntityDefinition definition)
        {
            if (!command.Publish || !definition.ForceUrlSlugUniqueness || SlugFormatter.ToSlug(dbVersion.Title) == dbVersion.CustomEntity.UrlSlug) return;

            var query = GetUniquenessQuery(command, definition, dbVersion);
            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("Cannot publish because the {1} '{0}' is not unique (symbols and spaces are ignored in the uniqueness check)",
                        command.Title,
                        definition.GetTerms().GetOrDefault(CustomizableCustomEntityTermKeys.Title, "title").ToLower());

                throw new UniqueConstraintViolationException(message, "Title", command.Title);
            }
        }

        private IsCustomEntityPathUniqueQuery GetUniquenessQuery(UpdateCustomEntityDraftVersionCommand command, ICustomEntityDefinition definition, CustomEntityVersion dbVersion)
        {
            var query = new IsCustomEntityPathUniqueQuery();
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

        private async Task<CustomEntityVersion> CreateDraftIfRequiredAsync(UpdateCustomEntityDraftVersionCommand command, CustomEntityVersion draft)
        {
            if (draft != null) return draft;

            var addDraftCommand = new AddCustomEntityDraftVersionCommand();
            addDraftCommand.CustomEntityId = command.CustomEntityId;
            await _commandExecutor.ExecuteAsync(addDraftCommand);

            return await GetDraftVersionAsync(command);
        }

        private void UpdateDraft(UpdateCustomEntityDraftVersionCommand command, CustomEntityVersion draft)
        {
            EntityNotFoundException.ThrowIfNull(draft, "Draft:" + command.CustomEntityId);

            draft.Title = command.Title.Trim();
            draft.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.Model);
        }

        #endregion

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
