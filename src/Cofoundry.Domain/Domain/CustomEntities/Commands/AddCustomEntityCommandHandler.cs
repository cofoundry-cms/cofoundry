using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Adds a new custom entity with a draft version and optionally publishes it.
/// </summary>
public class AddCustomEntityCommandHandler
    : ICommandHandler<AddCustomEntityCommand>
    , IPermissionRestrictedCommandHandler<AddCustomEntityCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;

    public AddCustomEntityCommandHandler(
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor,
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        ICustomEntityCache customEntityCache,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
        IMessageAggregator messageAggregator,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        ITransactionScopeManager transactionScopeFactory,
        ICustomEntityStoredProcedures customEntityStoredProcedures
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
        _transactionScopeFactory = transactionScopeFactory;
        _customEntityStoredProcedures = customEntityStoredProcedures;
    }

    public async Task ExecuteAsync(AddCustomEntityCommand command, IExecutionContext executionContext)
    {
        var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(command.CustomEntityDefinitionCode);
        var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
        EntityNotFoundException.ThrowIfNull(definition, command.CustomEntityDefinitionCode);

        await _commandExecutor.ExecuteAsync(new EnsureCustomEntityDefinitionExistsCommand(definition.CustomEntityDefinitionCode), executionContext);

        // Custom Validation
        var urlSlug = NormalizeUrlSlug(command, definition);
        ValidateCommand(command, definition);
        await ValidateIsUniqueAsync(urlSlug, command, definition, executionContext);

        var entity = MapEntity(urlSlug, command, definition, executionContext);

        await SetOrdering(entity, definition);

        _dbContext.CustomEntities.Add(entity);

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();

            var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                CustomEntityVersionEntityDefinition.DefinitionCode,
                entity.CustomEntityVersions.First().CustomEntityVersionId,
                command.Model);

            await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);
            await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(entity.CustomEntityId);

            scope.QueueCompletionTask(() => OnTransactionComplete(command, entity));

            await scope.CompleteAsync();
        }

        // Set Ouput
        command.OutputCustomEntityId = entity.CustomEntityId;
    }

    private Task OnTransactionComplete(
        AddCustomEntityCommand command,
        CustomEntity entity
        )
    {
        _customEntityCache.ClearRoutes(entity.CustomEntityDefinitionCode);

        return _messageAggregator.PublishAsync(new CustomEntityAddedMessage()
        {
            CustomEntityId = entity.CustomEntityId,
            CustomEntityDefinitionCode = entity.CustomEntityDefinitionCode,
            HasPublishedVersionChanged = command.Publish
        });
    }

    private string NormalizeUrlSlug(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
    {
        var value = definition.AutoGenerateUrlSlug ? command.Title : command.UrlSlug;
        return SlugFormatter.ToSlug(value);
    }

    private void ValidateCommand(AddCustomEntityCommand command, CustomEntityDefinitionSummary definition)
    {
        if (command.LocaleId.HasValue && !definition.HasLocale)
        {
            throw ValidationErrorException.CreateWithProperties(definition.NamePlural + " cannot be assigned locales", "LocaleId");
        }
    }

    private Locale? GetLocale(int? localeId)
    {
        if (!localeId.HasValue)
        {
            return null;
        }

        var locale = _dbContext
            .Locales
            .SingleOrDefault(l => l.LocaleId == localeId);

        if (locale == null)
        {
            throw ValidationErrorException.CreateWithProperties("The selected locale does not exist.", "LocaleId");
        }
        if (!locale.IsActive)
        {
            throw ValidationErrorException.CreateWithProperties("The selected locale is not active and cannot be used.", "LocaleId");
        }

        return locale;
    }

    private async Task SetOrdering(CustomEntity customEntity, CustomEntityDefinitionSummary definition)
    {
        if (definition.Ordering == CustomEntityOrdering.Full)
        {
            var maxOrdering = await _dbContext
                .CustomEntities
                .MaxAsync(e => e.Ordering);

            if (maxOrdering.HasValue)
            {
                // don't worry too much about race conditons here
                // if two entities are added at the same time it's no
                // big deal if the ordering is tied
                customEntity.Ordering = maxOrdering.Value + 1;
            }
            else
            {
                customEntity.Ordering = 0;
            }
        }
    }

    private CustomEntity MapEntity(
        string urlSlug,
        AddCustomEntityCommand command,
        CustomEntityDefinitionSummary definition,
        IExecutionContext executionContext
        )
    {
        var entity = new CustomEntity();
        _entityAuditHelper.SetCreated(entity, executionContext);

        entity.Locale = GetLocale(command.LocaleId);
        entity.UrlSlug = urlSlug;
        entity.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;

        var version = new CustomEntityVersion();
        version.Title = command.Title.Trim();
        version.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.Model) ?? string.Empty;
        version.DisplayVersion = 1;

        if (command.Publish)
        {
            entity.SetPublished(executionContext.ExecutionDate, command.PublishDate);
            version.WorkFlowStatusId = (int)WorkFlowStatus.Published;
        }
        else
        {
            entity.PublishStatusCode = PublishStatusCode.Unpublished;
            version.WorkFlowStatusId = (int)WorkFlowStatus.Draft;
        }

        _entityAuditHelper.SetCreated(version, executionContext);
        entity.CustomEntityVersions.Add(version);

        return entity;
    }

    private async Task ValidateIsUniqueAsync(
        string urlSlug,
        AddCustomEntityCommand command,
        CustomEntityDefinitionSummary definition,
        IExecutionContext executionContext
        )
    {
        if (!definition.ForceUrlSlugUniqueness)
        {
            return;
        }

        var isUnique = await _queryExecutor.ExecuteAsync(new IsCustomEntityUrlSlugUniqueQuery
        {
            CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode,
            LocaleId = command.LocaleId,
            UrlSlug = urlSlug
        }, executionContext);

        if (!isUnique)
        {
            string message;
            string prop;

            if (definition.AutoGenerateUrlSlug)
            {
                // If the slug is autogenerated then we should show the error with the title
                message = string.Format("The {1} '{0}' must be unique (symbols and spaces are ignored in the uniqueness check)",
                    command.Title,
                    definition.Terms.GetValueOrDefault(CustomizableCustomEntityTermKeys.Title, "title").ToLower());
                prop = "Title";
            }
            else
            {
                message = string.Format("The {1} '{0}' must be unique",
                    urlSlug,
                    definition.Terms.GetValueOrDefault(CustomizableCustomEntityTermKeys.UrlSlug, "url slug").ToLower());
                prop = "UrlSlug";
            }

            throw new UniqueConstraintViolationException(message, prop, command.UrlSlug);
        }
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddCustomEntityCommand command)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(command.CustomEntityDefinitionCode);
        yield return new CustomEntityCreatePermission(definition);

        if (command.Publish)
        {
            yield return new CustomEntityPublishPermission(definition);
        }
    }
}
