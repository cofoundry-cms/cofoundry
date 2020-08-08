using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    public class AddPageDirectoryCommandHandler
        : ICommandHandler<AddPageDirectoryCommand>
        , IPermissionRestrictedCommandHandler<AddPageDirectoryCommand>
    {
        #region constructor

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

        #endregion

        #region execution

        public async Task ExecuteAsync(AddPageDirectoryCommand command, IExecutionContext executionContext)
        {
            // Custom Validation
            await ValidateIsUniqueAsync(command, executionContext);

            var pageDirectory = new PageDirectory();
            pageDirectory.Name = command.Name;
            pageDirectory.UrlPath = command.UrlPath;
            pageDirectory.ParentPageDirectoryId = command.ParentPageDirectoryId;
            _entityAuditHelper.SetCreated(pageDirectory, executionContext);

            _dbContext.PageDirectories.Add(pageDirectory);
            await _dbContext.SaveChangesAsync();

            _transactionScopeFactory.QueueCompletionTask(_dbContext, _cache.Clear);

            command.OutputPageDirectoryId = pageDirectory.PageDirectoryId;
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

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageDirectoryCommand command)
        {
            yield return new PageDirectoryCreatePermission();
        }

        #endregion
    }
}
