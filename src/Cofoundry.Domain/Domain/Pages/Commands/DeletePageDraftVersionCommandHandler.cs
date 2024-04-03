using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Deletes the draft verison of a page version permanently if 
/// it exists. If no draft exists then no action is taken.
/// </summary>
public class DeletePageDraftVersionCommandHandler
    : ICommandHandler<DeletePageDraftVersionCommand>
    , IPermissionRestrictedCommandHandler<DeletePageDraftVersionCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IPageStoredProcedures _pageStoredProcedures;

    public DeletePageDraftVersionCommandHandler(
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
        _transactionScopeManager = transactionScopeManager;
        _pageStoredProcedures = pageStoredProcedures;
    }

    public async Task ExecuteAsync(DeletePageDraftVersionCommand command, IExecutionContext executionContext)
    {
        var draft = await _dbContext
            .PageVersions
            .FilterByPageId(command.PageId)
            .SingleOrDefaultAsync(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);

        if (draft != null)
        {
            var versionId = draft.PageVersionId;

            _dbContext.PageVersions.Remove(draft);

            using (var scope = _transactionScopeManager.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                scope.QueueCompletionTask(() => OnTransactionComplete(command, versionId));
                await scope.CompleteAsync();
            }
        }
    }

    private Task OnTransactionComplete(DeletePageDraftVersionCommand command, int versionId)
    {
        _pageCache.Clear(command.PageId);

        return _messageAggregator.PublishAsync(new PageDraftVersionDeletedMessage()
        {
            PageId = command.PageId,
            PageVersionId = versionId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDraftVersionCommand command)
    {
        yield return new PageUpdatePermission();
    }
}
