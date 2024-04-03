using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

public class AddPageDirectoryCommandHandler
    : ICommandHandler<AddPageDirectoryCommand>
    , IPermissionRestrictedCommandHandler<AddPageDirectoryCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly IPageDirectoryCache _cache;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IPageDirectoryStoredProcedures _pageDirectoryStoredProcedures;
    private readonly IMessageAggregator _messageAggregator;

    public AddPageDirectoryCommandHandler(
        IQueryExecutor queryExecutor,
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        IPageDirectoryCache cache,
        ITransactionScopeManager transactionScopeManager,
        IPageDirectoryStoredProcedures pageDirectoryStoredProcedures,
        IMessageAggregator messageAggregator
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
        _queryExecutor = queryExecutor;
        _cache = cache;
        _transactionScopeManager = transactionScopeManager;
        _pageDirectoryStoredProcedures = pageDirectoryStoredProcedures;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(AddPageDirectoryCommand command, IExecutionContext executionContext)
    {
        Normalize(command);
        var parentDirectory = await GetParentDirectoryAsync(command);
        await ValidateIsUniqueAsync(command, executionContext);

        var pageDirectory = new PageDirectory
        {
            Name = command.Name,
            UrlPath = command.UrlPath,
            ParentPageDirectory = parentDirectory
        };
        _entityAuditHelper.SetCreated(pageDirectory, executionContext);

        _dbContext.PageDirectories.Add(pageDirectory);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _pageDirectoryStoredProcedures.UpdatePageDirectoryClosureAsync();

            scope.QueueCompletionTask(() => OnTransactionComplete(pageDirectory.PageDirectoryId));
            await scope.CompleteAsync();
        }

        command.OutputPageDirectoryId = pageDirectory.PageDirectoryId;
    }

    private async Task OnTransactionComplete(int pageDirectoryId)
    {
        _cache.Clear();

        await _messageAggregator.PublishAsync(new PageDirectoryAddedMessage()
        {
            PageDirectoryId = pageDirectoryId
        });
    }

    private static void Normalize(AddPageDirectoryCommand command)
    {
        command.UrlPath = command.UrlPath.ToLowerInvariant();
        command.Name = command.Name?.Trim();
    }

    private async Task<PageDirectory> GetParentDirectoryAsync(AddPageDirectoryCommand command)
    {
        var parentDirectory = await _dbContext
            .PageDirectories
            .SingleOrDefaultAsync(d => d.PageDirectoryId == command.ParentPageDirectoryId);

        EntityNotFoundException.ThrowIfNull(parentDirectory, command.ParentPageDirectoryId);

        return parentDirectory;
    }

    private async Task ValidateIsUniqueAsync(AddPageDirectoryCommand command, IExecutionContext executionContext)
    {
        var query = new IsPageDirectoryPathUniqueQuery
        {
            ParentPageDirectoryId = command.ParentPageDirectoryId,
            UrlPath = command.UrlPath
        };

        var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

        if (!isUnique)
        {
            var message = $"A page directory already exists in that parent directory with the path '{command.UrlPath}'";
            throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
        }
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddPageDirectoryCommand command)
    {
        yield return new PageDirectoryCreatePermission();
    }
}
