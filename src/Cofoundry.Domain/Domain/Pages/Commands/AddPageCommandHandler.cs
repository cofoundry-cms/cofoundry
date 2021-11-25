using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AddPageCommandHandler
        : ICommandHandler<AddPageCommand>
        , IPermissionRestrictedCommandHandler<AddPageCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPageStoredProcedures _pageStoredProcedures;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public AddPageCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            IPageStoredProcedures pageStoredProcedures,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _pageStoredProcedures = pageStoredProcedures;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(AddPageCommand command, IExecutionContext executionContext)
        {
            Normalize(command);
            await ValidateIsPageUniqueAsync(command, executionContext);

            var page = await MapPage(command, executionContext);
            _dbContext.Pages.Add(page);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(page.PageId);

                scope.QueueCompletionTask(() => OnTransactionComplete(command, page));

                await scope.CompleteAsync();
            }

            // Set Ouput
            command.OutputPageId = page.PageId;
        }

        private Task OnTransactionComplete(AddPageCommand command, Page page)
        {
            _pageCache.ClearRoutes();

            return _messageAggregator.PublishAsync(new PageAddedMessage()
            {
                PageId = page.PageId,
                HasPublishedVersionChanged = command.Publish
            });
        }

        private void Normalize(AddPageCommand command)
        {
            command.UrlPath = command.UrlPath?.ToLowerInvariant();
            command.Title = command.Title.Trim();
            command.MetaDescription = command.MetaDescription?.Trim() ?? string.Empty;
            command.OpenGraphTitle = command.OpenGraphTitle?.Trim();
            command.OpenGraphDescription = command.OpenGraphDescription?.Trim();
        }

        private Locale GetLocale(int? localeId)
        {
            if (!localeId.HasValue) return null;

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

        private async Task<PageDirectory> GetPageDirectoryAsync(int pageDirectoryId)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .FilterById(pageDirectoryId)
                .SingleOrDefaultAsync();

            if (pageDirectory == null)
            {
                throw ValidationErrorException.CreateWithProperties("The selected page directory does not exist.", nameof(pageDirectory.PageDirectoryId));
            }

            return pageDirectory;
        }

        private async Task<Page> MapPage(AddPageCommand command, IExecutionContext executionContext)
        {
            // Create Page
            var page = new Page();
            page.PageTypeId = (int)command.PageType;
            page.Locale = GetLocale(command.LocaleId);
            page.PageDirectory = await GetPageDirectoryAsync(command.PageDirectoryId);

            _entityAuditHelper.SetCreated(page, executionContext);
            _entityTagHelper.UpdateTags(page.PageTags, command.Tags, executionContext);

            var pageTemplate = await GetTemplateAsync(command);

            if (command.PageType == PageType.CustomEntityDetails)
            {
                var definition = await GetCustomEntityDefinitionAsync(pageTemplate.CustomEntityDefinitionCode, executionContext);
                var rule = await GetAndValidateRoutingRuleAsync(command, definition, executionContext);

                page.CustomEntityDefinitionCode = pageTemplate.CustomEntityDefinitionCode;
                page.UrlPath = rule.RouteFormat;
            }
            else
            {
                page.UrlPath = command.UrlPath;
            }

            var pageVersion = new PageVersion();
            pageVersion.Title = command.Title;
            pageVersion.ExcludeFromSitemap = !command.ShowInSiteMap;
            pageVersion.MetaDescription = command.MetaDescription;
            pageVersion.OpenGraphTitle = command.OpenGraphTitle;
            pageVersion.OpenGraphDescription = command.OpenGraphDescription;
            pageVersion.OpenGraphImageId = command.OpenGraphImageId;
            pageVersion.PageTemplate = pageTemplate;
            pageVersion.DisplayVersion = 1;

            if (command.Publish)
            {
                page.PublishStatusCode = PublishStatusCode.Published;
                page.PublishDate = command.PublishDate ?? executionContext.ExecutionDate;
                pageVersion.WorkFlowStatusId = (int)WorkFlowStatus.Published;
            }
            else
            {
                page.PublishStatusCode = PublishStatusCode.Unpublished;
                page.PublishDate = command.PublishDate;
                pageVersion.WorkFlowStatusId = (int)WorkFlowStatus.Draft;
            }
            _entityAuditHelper.SetCreated(pageVersion, executionContext);
            page.PageVersions.Add(pageVersion);

            return page;
        }

        private async Task<ICustomEntityRoutingRule> GetAndValidateRoutingRuleAsync(
            AddPageCommand command,
            CustomEntityDefinitionSummary definition,
            IExecutionContext executionContext
            )
        {
            var rules = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityRoutingRulesQuery(), executionContext);
            var rule = rules.SingleOrDefault(r => r.RouteFormat == command.CustomEntityRoutingRule);

            if (rule == null)
            {
                throw ValidationErrorException.CreateWithProperties("Routing rule not found", "CustomEntityRoutingRule");
            }

            if (rule.RequiresUniqueUrlSlug && !definition.ForceUrlSlugUniqueness)
            {
                throw ValidationErrorException.CreateWithProperties("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", "CustomEntityRoutingRule");
            }

            return rule;
        }

        private async Task<CustomEntityDefinitionSummary> GetCustomEntityDefinitionAsync(string customEntityDefinitionCode, IExecutionContext executionContext)
        {
            var definition = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode), executionContext);
            if (definition == null)
            {
                throw ValidationErrorException.CreateWithProperties("Custom entity defintion does not exists.", nameof(definition.CustomEntityDefinitionCode));
            }
            return definition;
        }

        private async Task<PageTemplate> GetTemplateAsync(AddPageCommand command)
        {
            var template = await _dbContext
                .PageTemplates
                .SingleOrDefaultAsync(t => t.PageTemplateId == command.PageTemplateId);

            if (template == null)
            {
                throw ValidationErrorException.CreateWithProperties("Template not found.", nameof(command.PageTemplateId));
            }

            if (template.IsArchived)
            {
                throw ValidationErrorException.CreateWithProperties("You cannot use an archived template to create a new page.", nameof(command.PageTemplateId));
            }

            if (command.PageType == PageType.CustomEntityDetails)
            {
                if (!template.IsCustomEntityTemplate())
                {
                    throw ValidationErrorException.CreateWithProperties("Template does not support custom entities.", nameof(command.PageTemplateId));
                }
            }
            else if (template.IsCustomEntityTemplate())
            {
                throw ValidationErrorException.CreateWithProperties("A custom entity template template can only be used for custom entity details pages.", nameof(command.PageTemplateId));
            }

            return template;
        }

        private async Task ValidateIsPageUniqueAsync(AddPageCommand command, IExecutionContext executionContext)
        {
            string path;
            string propertyName;

            if (command.PageType == PageType.CustomEntityDetails)
            {
                path = command.CustomEntityRoutingRule;
                propertyName = nameof(command.CustomEntityRoutingRule);
            }
            else
            {
                path = command.UrlPath;
                propertyName = nameof(command.UrlPath);
            }

            var isUnique = await _queryExecutor.ExecuteAsync(new IsPagePathUniqueQuery()
            {
                LocaleId = command.LocaleId,
                PageDirectoryId = command.PageDirectoryId,
                UrlPath = path
            }, executionContext);

            if (!isUnique)
            {
                var message = $"A page already exists with the path '{path}' in that directory";
                throw new UniqueConstraintViolationException(message, propertyName, path);
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageCommand command)
        {
            yield return new PageCreatePermission();

            if (command.Publish)
            {
                yield return new PagePublishPermission();
            }
        }
    }
}
