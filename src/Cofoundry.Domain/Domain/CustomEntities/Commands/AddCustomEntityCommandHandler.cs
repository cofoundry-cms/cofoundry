using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.Validation;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class AddCustomEntityCommandHandler 
        : IAsyncCommandHandler<AddCustomEntityCommand>
        , IPermissionRestrictedCommandHandler<AddCustomEntityCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        
        public AddCustomEntityCommandHandler(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            ICustomEntityCache customEntityCache,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IMessageAggregator messageAggregator,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _customEntityCache = customEntityCache;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _messageAggregator = messageAggregator;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region Execution

        public async Task ExecuteAsync(AddCustomEntityCommand command, IExecutionContext executionContext)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(command.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, command.CustomEntityDefinitionCode);
            await _commandExecutor.ExecuteAsync(new EnsureCustomEntityDefinitionExistsCommand(definition.CustomEntityDefinitionCode));

            // Custom Validation
            ValidateCommand(command, definition);
            await ValidateIsUniqueAsync(command, definition);
            
            var entity = MapEntity(command, definition, executionContext);
            _dbContext.CustomEntities.Add(entity);

            using (var scope = _transactionScopeFactory.Create())
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionEntityDefinition.DefinitionCode,
                    entity.CustomEntityVersions.First().CustomEntityVersionId,
                    command.Model); 
                
                await _commandExecutor.ExecuteAsync(dependencyCommand);
                scope.Complete();
            }


            _customEntityCache.ClearRoutes(definition.CustomEntityDefinitionCode);

            // Set Ouput
            command.OutputCustomEntityId = entity.CustomEntityId;

            await _messageAggregator.PublishAsync(new CustomEntityAddedMessage()
            {
                CustomEntityId = entity.CustomEntityId,
                CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode,
                HasPublishedVersionChanged = command.Publish
            });
        }

        private void ValidateCommand(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
        {
            if (definition.AutoGenerateUrlSlug)
            {
                command.UrlSlug = SlugFormatter.ToSlug(command.Title);
            }
            else
            {
                command.UrlSlug = SlugFormatter.ToSlug(command.UrlSlug);
            }

            if (command.LocaleId.HasValue && !definition.HasLocale)
            {
                throw new PropertyValidationException(definition.NamePlural + " cannot be assigned locales", "LocaleId");
            }
        }
        
        private Locale GetLocale(int? localeId)
        {
            if (!localeId.HasValue) return null;

            var locale = _dbContext
                .Locales
                .SingleOrDefault(l => l.LocaleId == localeId);

            if (locale == null)
            {
                throw new PropertyValidationException("The selected locale does not exist.", "LocaleId");
            }
            if (!locale.IsActive)
            {
                throw new PropertyValidationException("The selected locale is not active and cannot be used.", "LocaleId");
            }

            return locale;
        }

        private IQueryable<CustomEntityDefinition> QueryDbDefinition(AddCustomEntityCommand command)
        {
            return _dbContext
                .CustomEntityDefinitions
                .Include(d => d.EntityDefinition)
                .Where(d => d.CustomEntityDefinitionCode == command.CustomEntityDefinitionCode);
        }

        private async Task<CustomEntityDefinition> GetAndValidateDbDefinitionAsync(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
        {
            var dbDefinition = await QueryDbDefinition(command).SingleOrDefaultAsync();

            // Registrastion is done via code, so if it doesnt exist in the db yet, lets add it
            if (dbDefinition == null)
            {
                await _commandExecutor.ExecuteAsync(new EnsureEntityDefinitionExistsCommand(command.CustomEntityDefinitionCode));
                dbDefinition = new CustomEntityDefinition()
                {
                    CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode,
                    ForceUrlSlugUniqueness = definition.ForceUrlSlugUniqueness,
                    HasLocale = definition.HasLocale,
                    IsOrderable = definition.Ordering != CustomEntityOrdering.None
                };
                _dbContext.CustomEntityDefinitions.Add(dbDefinition);
            }
            else
            {
                // update record
                dbDefinition.ForceUrlSlugUniqueness = definition.ForceUrlSlugUniqueness;
                dbDefinition.HasLocale = definition.HasLocale;
                dbDefinition.IsOrderable = definition.Ordering != CustomEntityOrdering.None;
            }

            return dbDefinition;
        }

        private CustomEntityDefinitionSummary GetCustomEntityDefinition(AddCustomEntityCommand command)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(command.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, command.CustomEntityDefinitionCode);
            return definition;
        }

        private CustomEntity MapEntity(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition, IExecutionContext executionContext)
        {
            // Create Page
            var entity = new CustomEntity();
            _entityAuditHelper.SetCreated(entity, executionContext);

            entity.Locale = GetLocale(command.LocaleId);
            entity.UrlSlug = command.UrlSlug;
            entity.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;

            var version = new CustomEntityVersion();
            version.Title = command.Title.Trim();
            version.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.Model);
            version.WorkFlowStatusId = command.Publish ? (int)WorkFlowStatus.Published : (int)WorkFlowStatus.Draft;
            
            _entityAuditHelper.SetCreated(version, executionContext);
            entity.CustomEntityVersions.Add(version);

            return entity;
        }

        #region uniqueness

        private async Task ValidateIsUniqueAsync(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
        {
            if (!definition.ForceUrlSlugUniqueness) return;

            var query = GetUniquenessQuery(command, definition);
            var isUnique = await _queryExecutor.ExecuteAsync(query);
            EnforceUniquenessResult(isUnique, command, definition);
        }

        private IsCustomEntityPathUniqueQuery GetUniquenessQuery(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
        {
            var query = new IsCustomEntityPathUniqueQuery();
            query.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
            query.LocaleId = command.LocaleId;
            query.UrlSlug = command.UrlSlug;

            return query;
        }

        private void EnforceUniquenessResult(bool isUnique, AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
        {
            if (!isUnique)
            {
                string message;
                string prop;

                if (definition.AutoGenerateUrlSlug)
                {
                    // If the slug is autogenerated then we should show the error with the title
                    message = string.Format("The {1} '{0}' must be unique (symbols and spaces are ignored in the uniqueness check)",
                        command.Title,
                        definition.Terms.GetOrDefault(CustomizableCustomEntityTermKeys.Title, "title").ToLower());
                    prop = "Title";
                }
                else
                {
                    message = string.Format("The {1} '{0}' must be unique", 
                        command.UrlSlug,
                        definition.Terms.GetOrDefault(CustomizableCustomEntityTermKeys.UrlSlug, "url slug").ToLower());
                    prop = "UrlSlug";
                }

                throw new UniqueConstraintViolationException(message, prop, command.UrlSlug);
            }
        }

        #endregion

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddCustomEntityCommand command)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            yield return new CustomEntityCreatePermission(definition);

            if (command.Publish)
            {
                yield return new CustomEntityPublishPermission(definition);
            }
        }

        #endregion
    }
}
