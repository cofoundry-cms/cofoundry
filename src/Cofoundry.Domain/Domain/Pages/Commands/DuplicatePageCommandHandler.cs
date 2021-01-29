using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal
{
    public class DuplicatePageCommandHandler 
        : ICommandHandler<DuplicatePageCommand>
        , IPermissionRestrictedCommandHandler<DuplicatePageCommand>
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageStoredProcedures _pageStoredProcedures;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DuplicatePageCommandHandler(
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            IPageStoredProcedures pageStoredProcedures,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _pageStoredProcedures = pageStoredProcedures;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DuplicatePageCommand command, IExecutionContext executionContext)
        {
            var pageToDuplicate = await GetPageToDuplicateAsync(command);
            var addPageCommand = MapCommand(command, pageToDuplicate);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _commandExecutor.ExecuteAsync(addPageCommand, executionContext);

                await _pageStoredProcedures.CopyBlocksToDraftAsync(
                    addPageCommand.OutputPageId,
                    pageToDuplicate.Version.PageVersionId,
                    executionContext.ExecutionDate,
                    executionContext.UserContext.UserId.Value);

                await scope.CompleteAsync();
            }

            // Set Ouput
            command.OutputPageId = addPageCommand.OutputPageId;
        }

        private class PageQuery
        {
            public int PageTypeId { get; set; }
            public PageVersion Version { get; set; }
            public ICollection<string> Tags { get; set; }
        }

        private Task<PageQuery> GetPageToDuplicateAsync(DuplicatePageCommand command)
        {
            return _dbContext
                .PageVersions
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(command.PageToDuplicateId)
                .OrderByLatest()
                .Select(v => new PageQuery
                {
                    PageTypeId = v.Page.PageTypeId,
                    Version = v,
                    Tags = v
                        .Page
                        .PageTags
                        .Select(t => t.Tag.TagText)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        private AddPageCommand MapCommand(DuplicatePageCommand command, PageQuery toDup)
        {
            EntityNotFoundException.ThrowIfNull(toDup, command.PageToDuplicateId);

            var addPageCommand = new AddPageCommand();
            addPageCommand.ShowInSiteMap = !toDup.Version.ExcludeFromSitemap;
            addPageCommand.PageTemplateId = toDup.Version.PageTemplateId;
            addPageCommand.MetaDescription = toDup.Version.MetaDescription;
            addPageCommand.OpenGraphDescription = toDup.Version.OpenGraphDescription;
            addPageCommand.OpenGraphImageId = toDup.Version.OpenGraphImageId;
            addPageCommand.OpenGraphTitle = toDup.Version.OpenGraphTitle;
            addPageCommand.PageType = (PageType)toDup.PageTypeId;

            addPageCommand.Title = command.Title;
            addPageCommand.LocaleId = command.LocaleId;
            addPageCommand.UrlPath = command.UrlPath;
            addPageCommand.CustomEntityRoutingRule = command.CustomEntityRoutingRule;
            addPageCommand.PageDirectoryId = command.PageDirectoryId;

            addPageCommand.Tags = toDup.Tags.ToArray();

            return addPageCommand;
        }

        #endregion

        #region Permissions

        public IEnumerable<IPermissionApplication> GetPermissions(DuplicatePageCommand command)
        {
            yield return new PageCreatePermission();
        }

        #endregion
    }
}
