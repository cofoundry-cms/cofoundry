using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
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
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IPageDirectoryCache cache,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _cache = cache;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(UpdatePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .FilterById(command.PageDirectoryId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(pageDirectory, command.PageDirectoryId);

            pageDirectory.Name = command.Name?.Trim();

            await _dbContext.SaveChangesAsync();
            _transactionScopeFactory.QueueCompletionTask(_dbContext, _cache.Clear);
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryCommand command)
        {
            yield return new PageDirectoryUpdatePermission();
        }
    }
}
