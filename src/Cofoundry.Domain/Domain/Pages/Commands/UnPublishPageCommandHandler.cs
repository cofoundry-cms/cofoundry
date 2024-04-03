using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Sets the status of a page to un-published, but does not
/// remove the publish date, which is preserved so that it
/// can be used as a default when the user chooses to publish
/// again.
/// </summary>
public class UnPublishPageCommandHandler
    : ICommandHandler<UnPublishPageCommand>
    , IPermissionRestrictedCommandHandler<UnPublishPageCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IPageStoredProcedures _pageStoredProcedures;

    public UnPublishPageCommandHandler(
        CofoundryDbContext dbContext,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeManager,
        IPageStoredProcedures pageStoredProcedures
        )
    {
        _dbContext = dbContext;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _pageStoredProcedures = pageStoredProcedures;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(UnPublishPageCommand command, IExecutionContext executionContext)
    {
        var page = await _dbContext
            .Pages
            .FilterActive()
            .FilterById(command.PageId)
            .SingleOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(page, command.PageId);

        if (page.PublishStatusCode == PublishStatusCode.Unpublished)
        {
            // No action
            return;
        }

        var version = await _dbContext
            .PageVersions
            .Include(p => p.Page)
            .FilterActive()
            .FilterByPageId(command.PageId)
            .OrderByLatest()
            .FirstOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(version, command.PageId);

        page.PublishStatusCode = PublishStatusCode.Unpublished;
        version.WorkFlowStatusId = (int)WorkFlowStatus.Draft;

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

            scope.QueueCompletionTask(() => OnTransactionComplete(command));

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(UnPublishPageCommand command)
    {
        _pageCache.Clear();

        return _messageAggregator.PublishAsync(new PageUnPublishedMessage()
        {
            PageId = command.PageId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UnPublishPageCommand command)
    {
        yield return new PagePublishPermission();
    }
}
