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
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPageStoredProcedures _pageStoredProcedures;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public AddPageDraftVersionCommandHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        IPageStoredProcedures pageStoredProcedures,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _pageStoredProcedures = pageStoredProcedures;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext)
    {
        var user = _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);

        int newVersionId;

        try
        {
            newVersionId = await _pageStoredProcedures.AddDraftAsync(
                command.PageId,
                command.CopyFromPageVersionId,
                executionContext.ExecutionDate,
                user.UserId
                );
        }
        catch (StoredProcedureExecutionException ex) when (ex.ErrorNumber == StoredProcedureErrorNumbers.Page_AddDraft.DraftAlreadyExists)
        {
            throw ValidationErrorException.CreateWithProperties("A draft cannot be created because this page already has one.", nameof(command.PageId));
        }

        await _transactionScopeManager.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, newVersionId));

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
