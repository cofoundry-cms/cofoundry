using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UpdatePageUrlCommandHandler
    : ICommandHandler<UpdatePageUrlCommand>
    , IPermissionRestrictedCommandHandler<UpdatePageUrlCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public UpdatePageUrlCommandHandler(
        IQueryExecutor queryExecutor,
        CofoundryDbContext dbContext,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _queryExecutor = queryExecutor;
        _dbContext = dbContext;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(UpdatePageUrlCommand command, IExecutionContext executionContext)
    {
        Normalize(command);

        var page = await _dbContext
            .Pages
            .Include(p => p.Locale)
            .Include(p => p.PageDirectory)
            .FilterActive()
            .FilterById(command.PageId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(page, command.PageId);

        ValidateUrlParameters(command, page);
        await ValidateIsPageUniqueAsync(command, page, executionContext);

        var originalPageRoute = await _queryExecutor.ExecuteAsync(new GetPageRouteByIdQuery(page.PageId));
        EntityNotFoundException.ThrowIfNull(originalPageRoute, page.PageId);

        await MapPageAsync(command, executionContext, page);
        var isPublished = page.PublishStatusCode == PublishStatusCode.Published;

        await _dbContext.SaveChangesAsync();
        await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(originalPageRoute, isPublished));
    }

    private Task OnTransactionComplete(PageRoute oldPageRoute, bool isPublished)
    {
        _pageCache.Clear(oldPageRoute.PageId);

        return _messageAggregator.PublishAsync(new PageUrlChangedMessage()
        {
            PageId = oldPageRoute.PageId,
            OldFullUrlPath = oldPageRoute.FullUrlPath,
            HasPublishedVersionChanged = isPublished
        });
    }

    private static void Normalize(UpdatePageUrlCommand command)
    {
        command.UrlPath = command.UrlPath?.ToLowerInvariant();
    }

    private static void ValidateUrlParameters(UpdatePageUrlCommand command, Page page)
    {
        var pageType = (PageType)page.PageTypeId;

        if (pageType == PageType.CustomEntityDetails)
        {
            if (string.IsNullOrWhiteSpace(command.CustomEntityRoutingRule))
            {
                throw ValidationErrorException.CreateWithProperties(
                    "A routing rule is required for custom entity details page types.",
                    nameof(command.CustomEntityRoutingRule)
                    );
            }
            if (!string.IsNullOrEmpty(command.UrlPath))
            {
                throw ValidationErrorException.CreateWithProperties(
                    "Custom entity details pages should not specify a Url Path, instead they should specify a Routing Rule.",
                    nameof(command.UrlPath)
                    );
            }
        }

        if (pageType != PageType.CustomEntityDetails && !string.IsNullOrEmpty(command.CustomEntityRoutingRule))
        {
            throw ValidationErrorException.CreateWithProperties(
                "Custom Entity routing rules should only be specified for custom entity details page types.",
                nameof(command.CustomEntityRoutingRule)
                );
        }
    }

    private async Task ValidateIsPageUniqueAsync(UpdatePageUrlCommand command, Page page, IExecutionContext executionContext)
    {
        var path = command.UrlPath;
        var propertyName = nameof(command.UrlPath);

        if (page.PageTypeId == (int)PageType.CustomEntityDetails)
        {
            path = command.CustomEntityRoutingRule;
            propertyName = nameof(command.CustomEntityRoutingRule);
        }

        if (path == null)
        {
            throw new InvalidOperationException($"{nameof(path)} should have already been validfated not to be null.");
        }

        var isUnique = await _queryExecutor.ExecuteAsync(new IsPagePathUniqueQuery()
        {
            PageId = page.PageId,
            LocaleId = command.LocaleId,
            UrlPath = path,
            PageDirectoryId = command.PageDirectoryId
        }, executionContext);

        if (!isUnique)
        {
            var message = string.Format("A page already exists with the path '{0}' in that directory", path);
            throw new UniqueConstraintViolationException(message, propertyName, path);
        }
    }

    private async Task MapPageAsync(UpdatePageUrlCommand command, IExecutionContext executionContext, Page page)
    {
        if (page.PageTypeId == (int)PageType.CustomEntityDetails)
        {
            ArgumentNullException.ThrowIfNull(command.CustomEntityRoutingRule);
            var rule = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutingRuleByRouteFormatQuery(command.CustomEntityRoutingRule), executionContext);
            if (rule == null)
            {
                throw ValidationErrorException.CreateWithProperties("Routing rule not found", nameof(command.CustomEntityRoutingRule));
            }

            if (page.CustomEntityDefinitionCode == null)
            {
                throw new InvalidOperationException($"{nameof(page)}.{nameof(page.CustomEntityDefinitionCode)} should not be null for a custom entity details page.");
            }

            var customEntityDefinition = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery()
            {
                CustomEntityDefinitionCode = page.CustomEntityDefinitionCode
            }, executionContext);
            EntityNotFoundException.ThrowIfNull(customEntityDefinition, page.CustomEntityDefinitionCode);

            if (customEntityDefinition.ForceUrlSlugUniqueness && !rule.RequiresUniqueUrlSlug)
            {
                throw ValidationErrorException.CreateWithProperties("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", nameof(command.CustomEntityRoutingRule));
            }

            page.UrlPath = rule.RouteFormat;
        }
        else
        {
            ArgumentNullException.ThrowIfNull(command.UrlPath);
            page.UrlPath = command.UrlPath;
        }

        page.PageDirectoryId = command.PageDirectoryId;
        page.LocaleId = command.LocaleId;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageUrlCommand command)
    {
        yield return new PageUpdateUrlPermission();
    }
}
