using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

public class DeletePageCommandHandler
    : ICommandHandler<DeletePageCommand>
    , IPermissionRestrictedCommandHandler<DeletePageCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IPageStoredProcedures _pageStoredProcedures;
    private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;

    public DeletePageCommandHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeFactory,
        IPageStoredProcedures pageStoredProcedures,
        IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _transactionScopeFactory = transactionScopeFactory;
        _pageStoredProcedures = pageStoredProcedures;
        _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
    }

    public async Task ExecuteAsync(DeletePageCommand command, IExecutionContext executionContext)
    {
        var page = await _dbContext
            .Pages
            .FilterById(command.PageId)
            .SingleOrDefaultAsync();

        if (page == null) return;

        var pageRoute = await _queryExecutor.ExecuteAsync(new GetPageRouteByIdQuery(page.PageId), executionContext);
        EntityNotFoundException.ThrowIfNull(pageRoute, command.PageId);

        await _dependableEntityDeleteCommandValidator.ValidateAsync(PageEntityDefinition.DefinitionCode, command.PageId, executionContext);

        _dbContext.Pages.Remove(page);

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

            scope.QueueCompletionTask(() => OnTransactionComplete(pageRoute));
            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(PageRoute page)
    {
        _pageCache.Clear(page.PageId);

        return _messageAggregator.PublishAsync(new PageDeletedMessage()
        {
            PageId = page.PageId,
            FullUrlPath = page.FullUrlPath
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DeletePageCommand command)
    {
        yield return new PageDeletePermission();
    }
}
