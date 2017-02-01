using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.Validation;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdatePageUrlCommandHandler 
        : IAsyncCommandHandler<UpdatePageUrlCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageUrlCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;

        public UpdatePageUrlCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdatePageUrlCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .FilterById(command.PageId)
                .Include(p => p.Locale)
                .Include(p => p.WebDirectory)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(page, command.PageId);

            await ValidateIsPageUniqueAsync(command, page, executionContext);

            MapPage(command, executionContext, page);
            var isPublished = page.PageVersions.Any(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);

            await _dbContext.SaveChangesAsync();
            _pageCache.Clear(command.PageId);

            await _messageAggregator.PublishAsync(new PageUrlChangedMessage()
            {
                PageId = command.PageId,
                HasPublishedVersionChanged = isPublished
            });
        }

        #endregion

        #region helpers

        private void MapPage(UpdatePageUrlCommand command, IExecutionContext executionContext, Page page)
        {
            if (page.PageTypeId == (int)PageType.CustomEntityDetails)
            {
                var rule = _queryExecutor.Execute(new GetCustomEntityRoutingRuleByRouteFormatQuery(command.CustomEntityRoutingRule), executionContext);
                if (rule == null)
                {
                    throw new PropertyValidationException("Routing rule not found", "CustomEntityRoutingRule", command.CustomEntityRoutingRule);
                }

                var customEntityDefinition = _queryExecutor.Execute(new GetByStringQuery<CustomEntityDefinitionSummary>() { Id = page.CustomEntityDefinitionCode }, executionContext);
                EntityNotFoundException.ThrowIfNull(customEntityDefinition, page.CustomEntityDefinitionCode);

                if (customEntityDefinition.ForceUrlSlugUniqueness && !rule.RequiresUniqueUrlSlug)
                {
                    throw new PropertyValidationException("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", "CustomEntityRoutingRule", command.CustomEntityRoutingRule);
                }

                page.UrlPath = rule.RouteFormat;
            }
            else
            {
                page.UrlPath = command.UrlPath;
            }

            page.WebDirectoryId = command.WebDirectoryId;
            page.LocaleId = command.LocaleId;
        }

        private async Task ValidateIsPageUniqueAsync(UpdatePageUrlCommand command, Page page, IExecutionContext executionContext)
        {
            var query = GetUniquenessQuery(command, page);
            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);
            ValidateUnique(command, isUnique);
        }

        private IsPagePathUniqueQuery GetUniquenessQuery(UpdatePageUrlCommand command, Page page)
        {
            var query = new IsPagePathUniqueQuery();
            query.PageId = page.PageId;
            query.LocaleId = command.LocaleId;
            if (page.PageTypeId == (int)PageType.CustomEntityDetails)
            {
                query.UrlPath = command.UrlPath;
            }
            else
            {
                query.UrlPath = command.CustomEntityRoutingRule;
            }

            query.WebDirectoryId = command.WebDirectoryId;

            return query;
        }

        private void ValidateUnique(UpdatePageUrlCommand command, bool isUnique)
        {
            if (!isUnique)
            {
                var message = string.Format("A page already exists with the path '{0}' in that directory", command.UrlPath);
                throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
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
