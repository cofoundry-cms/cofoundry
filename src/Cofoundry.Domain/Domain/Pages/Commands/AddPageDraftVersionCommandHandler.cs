using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Creates a new draft version of a page from the currently published version. If there
/// isn't a currently published version then an exception will be thrown. An exception is also 
/// thrown if there is already a draft version.
/// </summary>
public class AddPageDraftVersionCommandHandler
    : ICommandHandler<AddPageDraftVersionCommand>
    , IPermissionRestrictedCommandHandler<AddPageDraftVersionCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPageStoredProcedures _pageStoredProcedures;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public AddPageDraftVersionCommandHandler(
        CofoundryDbContext dbContext,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        IPageStoredProcedures pageStoredProcedures,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _pageStoredProcedures = pageStoredProcedures;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext)
    {
        int newVersionId;

        try
        {
            newVersionId = await _pageStoredProcedures.AddDraftAsync(
                command.PageId,
                command.CopyFromPageVersionId,
                executionContext.ExecutionDate,
                executionContext.UserContext.UserId.Value);
        }
        catch (StoredProcedureExecutionException ex) when (ex.ErrorNumber == StoredProcedureErrorNumbers.Page_AddDraft.DraftAlreadyExists)
        {
            throw ValidationErrorException.CreateWithProperties("A draft cannot be created because this page already has one.", nameof(command.PageId));
        }

        await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, newVersionId));

        command.OutputPageVersionId = newVersionId;
    }

    private Task OnTransactionComplete(AddPageDraftVersionCommand command, int newVersionId)
    {
        _pageCache.Clear(command.PageId);

        return _messageAggregator.PublishAsync(new PageDraftVersionAddedMessage()
        {
            PageId = command.PageId,
            PageVersionId = newVersionId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddPageDraftVersionCommand command)
    {
        yield return new PageUpdatePermission();
    }
}
