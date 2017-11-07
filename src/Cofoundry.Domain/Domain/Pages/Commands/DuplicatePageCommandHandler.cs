using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DuplicatePageCommandHandler 
        : IAsyncCommandHandler<DuplicatePageCommand>
        , IPermissionRestrictedCommandHandler<DuplicatePageCommand>
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityTagHelper _entityTagHelper;

        public DuplicatePageCommandHandler(
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            EntityTagHelper entityTagHelper
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _entityTagHelper = entityTagHelper;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DuplicatePageCommand command, IExecutionContext executionContext)
        {
            var toDup = await GetPageToDuplicate(command).FirstOrDefaultAsync();
            var addPageCommand = MapCommand(command, toDup);

            await _commandExecutor.ExecuteAsync(addPageCommand);

            // Set Ouput
            command.OutputPageId = addPageCommand.OutputPageId;
        }

        #endregion

        #region helpers

        private class PageQuery
        {
            public int PageTypeId { get; set; }
            public PageVersion Version { get; set; }
            public IEnumerable<string> Tags { get; set; }
        }

        private IQueryable<PageQuery> GetPageToDuplicate(DuplicatePageCommand command)
        {
            return _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => v.PageId == command.PageToDuplicateId && !v.IsDeleted)
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(v => v.CreateDate)
                .Select(v => new PageQuery
                {
                    PageTypeId = v.Page.PageTypeId,
                    Version = v,
                    Tags = v.Page.PageTags.Select(t => t.Tag.TagText)
                });
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
