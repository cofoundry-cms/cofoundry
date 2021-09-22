using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AddPageDirectoryCommandHandler
        : ICommandHandler<AddPageDirectoryCommand>
        , IPermissionRestrictedCommandHandler<AddPageDirectoryCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageDirectoryCache _cache;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public AddPageDirectoryCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IPageDirectoryCache cache,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _queryExecutor = queryExecutor;
            _cache = cache;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(AddPageDirectoryCommand command, IExecutionContext executionContext)
        {
            var parentDirectory = await GetParentDirectoryAsync(command);
            await ValidateIsUniqueAsync(command, executionContext);

            var pageDirectory = new PageDirectory();
            pageDirectory.Name = command.Name;
            pageDirectory.UrlPath = command.UrlPath;
            pageDirectory.ParentPageDirectory = parentDirectory;
            _entityAuditHelper.SetCreated(pageDirectory, executionContext);

            _dbContext.PageDirectories.Add(pageDirectory);
            await _dbContext.SaveChangesAsync();

            _transactionScopeFactory.QueueCompletionTask(_dbContext, _cache.Clear);

            command.OutputPageDirectoryId = pageDirectory.PageDirectoryId;
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
            var query = new IsPageDirectoryPathUniqueQuery();
            query.ParentPageDirectoryId = command.ParentPageDirectoryId;
            query.UrlPath = command.UrlPath;

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
}
