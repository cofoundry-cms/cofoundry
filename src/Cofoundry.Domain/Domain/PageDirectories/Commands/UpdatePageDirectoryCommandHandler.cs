using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the main properties of an existing page directory. To
/// update properties that affect the route, use <see cref="UpdatePageDirectoryUrlCommand"/>.
/// </summary>
public class UpdatePageDirectoryCommandHandler
    : ICommandHandler<UpdatePageDirectoryCommand>
    , IPermissionRestrictedCommandHandler<UpdatePageDirectoryCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageDirectoryCache _cache;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public UpdatePageDirectoryCommandHandler(
        CofoundryDbContext dbContext,
        IPageDirectoryCache cache,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _cache = cache;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(UpdatePageDirectoryCommand command, IExecutionContext executionContext)
    {
        Normalize(command);

        var pageDirectory = await _dbContext
            .PageDirectories
            .FilterById(command.PageDirectoryId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(pageDirectory, command.PageDirectoryId);

        pageDirectory.Name = command.Name;

        await _dbContext.SaveChangesAsync();
        _transactionScopeManager.QueueCompletionTask(_dbContext, _cache.Clear);
    }

    private static void Normalize(UpdatePageDirectoryCommand command)
    {
        command.Name = command.Name?.Trim();
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryCommand command)
    {
        yield return new PageDirectoryUpdatePermission();
    }
}
