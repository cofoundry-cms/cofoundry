using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UpdatePageUrlCommandHandler 
        : ICommandHandler<UpdatePageUrlCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageUrlCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageUrlCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdatePageUrlCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .Include(p => p.Locale)
                .Include(p => p.PageDirectory)
                .FilterActive()
                .FilterByPageId(command.PageId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(page, command.PageId);

            await ValidateIsPageUniqueAsync(command, page, executionContext);

            await MapPageAsync(command, executionContext, page);
            var isPublished = page.PublishStatusCode == PublishStatusCode.Published;

            await _dbContext.SaveChangesAsync();

            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, isPublished));
        }

        private Task OnTransactionComplete(UpdatePageUrlCommand command, bool isPublished)
        {
            _pageCache.Clear(command.PageId);

            return _messageAggregator.PublishAsync(new PageUrlChangedMessage()
            {
                PageId = command.PageId,
                HasPublishedVersionChanged = isPublished
            });
        }

        #endregion

        #region helpers

        private async Task MapPageAsync(UpdatePageUrlCommand command, IExecutionContext executionContext, Page page)
        {
            if (page.PageTypeId == (int)PageType.CustomEntityDetails)
            {
                var rule = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutingRuleByRouteFormatQuery(command.CustomEntityRoutingRule), executionContext);
                if (rule == null)
                {
                    throw ValidationErrorException.CreateWithProperties("Routing rule not found", "CustomEntityRoutingRule");
                }

                var customEntityDefinition = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(page.CustomEntityDefinitionCode), executionContext);
                EntityNotFoundException.ThrowIfNull(customEntityDefinition, page.CustomEntityDefinitionCode);

                if (customEntityDefinition.ForceUrlSlugUniqueness && !rule.RequiresUniqueUrlSlug)
                {
                    throw ValidationErrorException.CreateWithProperties("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", "CustomEntityRoutingRule");
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
            var query = GetUniquenessQuery(command, page);
            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);
            ValidateUnique(command, page, isUnique);
        }

        private IsPagePathUniqueQuery GetUniquenessQuery(UpdatePageUrlCommand command, Page page)
        {
            var query = new IsPagePathUniqueQuery();
            query.PageId = page.PageId;
            query.LocaleId = command.LocaleId;

            query.UrlPath = GetUrlPath(command, page);
            query.PageDirectoryId = command.PageDirectoryId;

            return query;
        }

        private static string GetUrlPath(UpdatePageUrlCommand command, Page page)
        {
            string urlPath;
            if (page.PageTypeId == (int)PageType.CustomEntityDetails)
            {
                urlPath = command.UrlPath;
            }
            else
            {
                urlPath = command.CustomEntityRoutingRule;
            }

            return urlPath;
        }

        private void ValidateUnique(UpdatePageUrlCommand command, Page page, bool isUnique)
        {
            if (!isUnique)
            {
                var path = GetUrlPath(command, page);
                var message = string.Format("A page already exists with the path '{0}' in that directory", path);
                throw new UniqueConstraintViolationException(message, "UrlPath", path);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageUrlCommand command)
        {
            yield return new PageUpdateUrlPermission();
        }

        #endregion
    }
}
