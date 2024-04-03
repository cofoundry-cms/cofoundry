using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

public class DuplicatePageCommandHandler
    : ICommandHandler<DuplicatePageCommand>
    , IPermissionRestrictedCommandHandler<DuplicatePageCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageStoredProcedures _pageStoredProcedures;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public DuplicatePageCommandHandler(
        ICommandExecutor commandExecutor,
        IPermissionValidationService permissionValidationService,
        CofoundryDbContext dbContext,
        IPageStoredProcedures pageStoredProcedures,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _commandExecutor = commandExecutor;
        _permissionValidationService = permissionValidationService;
        _dbContext = dbContext;
        _pageStoredProcedures = pageStoredProcedures;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(DuplicatePageCommand command, IExecutionContext executionContext)
    {
        var user = _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);
        var pageToDuplicate = await GetPageToDuplicateAsync(command);
        var addPageCommand = MapCommand(command, pageToDuplicate);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _commandExecutor.ExecuteAsync(addPageCommand, executionContext);

            await _pageStoredProcedures.CopyBlocksToDraftAsync(
                addPageCommand.OutputPageId,
                pageToDuplicate.Version.PageVersionId,
                executionContext.ExecutionDate,
                user.UserId
                );

            await scope.CompleteAsync();
        }

        // Set Ouput
        command.OutputPageId = addPageCommand.OutputPageId;
    }

    private class PageQuery
    {
        public required int PageTypeId { get; set; }
        public required PageVersion Version { get; set; }
        public required IReadOnlyCollection<string> Tags { get; set; }
    }

    private async Task<PageQuery> GetPageToDuplicateAsync(DuplicatePageCommand command)
    {
        var dbResult = await _dbContext
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

        EntityNotFoundException.ThrowIfNull(dbResult, command.PageToDuplicateId);

        return dbResult;

    }

    private static AddPageCommand MapCommand(DuplicatePageCommand command, PageQuery toDup)
    {
        var addPageCommand = new AddPageCommand
        {
            ShowInSiteMap = !toDup.Version.ExcludeFromSitemap,
            PageTemplateId = toDup.Version.PageTemplateId,
            MetaDescription = toDup.Version.MetaDescription,
            OpenGraphDescription = toDup.Version.OpenGraphDescription,
            OpenGraphImageId = toDup.Version.OpenGraphImageId,
            OpenGraphTitle = toDup.Version.OpenGraphTitle,
            PageType = (PageType)toDup.PageTypeId,
            Title = command.Title,
            LocaleId = command.LocaleId,
            UrlPath = command.UrlPath,
            CustomEntityRoutingRule = command.CustomEntityRoutingRule,
            PageDirectoryId = command.PageDirectoryId,
            Tags = toDup.Tags
        };

        return addPageCommand;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DuplicatePageCommand command)
    {
        yield return new PageCreatePermission();
    }
}
