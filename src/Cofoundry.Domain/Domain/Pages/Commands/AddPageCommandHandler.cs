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

namespace Cofoundry.Domain
{
    public class AddPageCommandHandler 
        : IAsyncCommandHandler<AddPageCommand>
        , IPermissionRestrictedCommandHandler<AddPageCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        
        public AddPageCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(AddPageCommand command, IExecutionContext executionContext)
        {
            // Custom Validation
            await ValidateIsPageUniqueAsync(command, executionContext);

            var page = await MapPage(command, executionContext);
            _dbContext.Pages.Add(page);
            await _dbContext.SaveChangesAsync();

            _pageCache.ClearRoutes();

            // Set Ouput
            command.OutputPageId = page.PageId;

            await _messageAggregator.PublishAsync(new PageAddedMessage()
            {
                PageId = page.PageId,
                HasPublishedVersionChanged = command.Publish
            });
        }

        #endregion

        #region helpers

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

        private WebDirectory GetWebDirectory(int webDirectoryId)
        {
            var webDirectory = _dbContext
                .WebDirectories
                .Include(w => w.ParentWebDirectory)
                .SingleOrDefault(w => w.WebDirectoryId == webDirectoryId);

            if (webDirectory == null)
            {
                throw new PropertyValidationException("The selected web directory does not exist.", "WebDirectoryId");
            }
            CheckWebDirectoryIsActive(webDirectory);

            return webDirectory;
        }

        private void CheckWebDirectoryIsActive(WebDirectory webDirectory)
        {
            if (!webDirectory.IsActive)
            {
                throw new PropertyValidationException("The selected web directory is not active and cannot be used.", "WebDirectoryId");
            }
            if (webDirectory.ParentWebDirectoryId.HasValue)
            {
                CheckWebDirectoryIsActive(webDirectory.ParentWebDirectory);
            }
        }

        private async Task<Page> MapPage(AddPageCommand command, IExecutionContext executionContext)
        {
            // Create Page
            var page = new Page();
            page.PageTypeId = (int)command.PageType;
            page.Locale = GetLocale(command.LocaleId);
            page.WebDirectory = GetWebDirectory(command.WebDirectoryId);
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
            pageVersion.MetaDescription = command.MetaDescription ?? string.Empty;
            pageVersion.WorkFlowStatusId = command.Publish ? (int)WorkFlowStatus.Published : (int)WorkFlowStatus.Draft;
            pageVersion.OpenGraphTitle = command.OpenGraphTitle;
            pageVersion.OpenGraphDescription = command.OpenGraphDescription;
            pageVersion.OpenGraphImageId = command.OpenGraphImageId;
            pageVersion.PageTemplate = pageTemplate;

            _entityAuditHelper.SetCreated(pageVersion, executionContext);
            page.PageVersions.Add(pageVersion);

            return page;
        }

        private async Task<ICustomEntityRoutingRule> GetAndValidateRoutingRuleAsync(AddPageCommand command, CustomEntityDefinitionSummary definition, IExecutionContext ex)
        {
            var rules = await _queryExecutor.ExecuteAsync(new GetAllQuery<ICustomEntityRoutingRule>(), ex);
            var rule = rules.SingleOrDefault(r => r.RouteFormat == command.CustomEntityRoutingRule);

            if (rule == null)
            {
                throw new PropertyValidationException("Routing rule not found", "CustomEntityRoutingRule", command.CustomEntityRoutingRule);
            }

            if (rule.RequiresUniqueUrlSlug && !definition.ForceUrlSlugUniqueness)
            {
                throw new PropertyValidationException("Ths routing rule requires a unique url slug, but the selected custom entity does not enforce url slug uniqueness", "CustomEntityRoutingRule");
            }

            return rule;
        }

        private async Task<CustomEntityDefinitionSummary> GetCustomEntityDefinitionAsync(string customEntityDefinitionCode, IExecutionContext ex)
        {
            var definition = await _queryExecutor.ExecuteAsync(new GetByStringQuery<CustomEntityDefinitionSummary>() { Id = customEntityDefinitionCode }, ex);
            if (definition == null)
            {
                throw new PropertyValidationException("Custom entity defintion does not exists", "CustomEntityDefinitionCode", customEntityDefinitionCode);
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
                throw new PropertyValidationException("Template not found", "PageTemplateId");
            }

            if (command.PageType == PageType.CustomEntityDetails)
            {
                if (!template.IsCustomEntityTemplate())
                {
                    throw new PropertyValidationException("Template does not support custom entities", "PageTemplateId");
                }
            }

            return template;
        }

        #region uniqueness

        private async Task ValidateIsPageUniqueAsync(AddPageCommand command, IExecutionContext ex)
        {
            var query = CreateUniquenessQuery(command);
            var isUnique = await _queryExecutor.ExecuteAsync(query, ex);
            ValidateIsUnique(command, isUnique);
        }

        private IsPagePathUniqueQuery CreateUniquenessQuery(AddPageCommand command)
        {
            var query = new IsPagePathUniqueQuery();
            query.LocaleId = command.LocaleId;
            query.WebDirectoryId = command.WebDirectoryId;

            if (command.PageType == PageType.CustomEntityDetails)
            {
                query.UrlPath = command.CustomEntityRoutingRule;
            }
            else
            {
                query.UrlPath = command.UrlPath;
            }

            return query;
        }

        private void ValidateIsUnique(AddPageCommand command, bool isUnique)
        {
            if (!isUnique)
            {
                var message = string.Format("A page already exists with the path '{0}' in that directory", command.UrlPath);
                throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
            }
        }

        #endregion

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageCommand command)
        {
            yield return new PageCreatePermission();

            if (command.Publish)
            {
                yield return new PagePublishPermission();
            }
        }

        #endregion
    }
}
