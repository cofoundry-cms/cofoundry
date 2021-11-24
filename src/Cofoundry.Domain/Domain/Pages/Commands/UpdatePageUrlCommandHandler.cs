using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
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
            var page = await _dbContext
                .Pages
                .Include(p => p.Locale)
                .Include(p => p.PageDirectory)
                .FilterActive()
                .FilterById(command.PageId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(page, command.PageId);

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
                OldFullUrlPath = oldPageRoute.FullPath,
                HasPublishedVersionChanged = isPublished
            });
        }

        private async Task MapPageAsync(UpdatePageUrlCommand command, IExecutionContext executionContext, Page page)
        {
            if (page.PageTypeId == (int)PageType.CustomEntityDetails)
            {
                var rule = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutingRuleByRouteFormatQuery(command.CustomEntityRoutingRule), executionContext);
                if (rule == null)
                {
                    throw ValidationErrorException.CreateWithProperties("Routing rule not found", nameof(command.CustomEntityRoutingRule));
                }

                var customEntityDefinition = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(page.CustomEntityDefinitionCode), executionContext);
                EntityNotFoundException.ThrowIfNull(customEntityDefinition, page.CustomEntityDefinitionCode);

                if (customEntityDefinition.ForceUrlSlugUniqueness && !rule.RequiresUniqueUrlSlug)
                {
                    throw ValidationErrorException.CreateWithProperties("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", nameof(command.CustomEntityRoutingRule));
                }

                page.UrlPath = rule.RouteFormat;
            }
            else
            {
                page.UrlPath = command.UrlPath;
            }

            page.PageDirectoryId = command.PageDirectoryId;
            page.LocaleId = command.LocaleId;
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

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageUrlCommand command)
        {
            yield return new PageUpdateUrlPermission();
        }
    }
}
