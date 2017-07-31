using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class DeletePageDirectoryCommandHandler
        : IAsyncCommandHandler<DeletePageDirectoryCommand>
        , IPermissionRestrictedCommandHandler<DeletePageDirectoryCommand>
    {
        #region constructor 

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageDirectoryCache _cache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly ICommandExecutor _commandExecutor;

        public DeletePageDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IPageDirectoryCache cache,
            ICommandExecutor commandExecutor,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _cache = cache;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DeletePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .Include(d => d.ChildPageDirectories)
                .SingleOrDefaultAsync(d => d.PageDirectoryId == command.PageDirectoryId);

            if (pageDirectory != null)
            {
                if (!pageDirectory.ParentPageDirectoryId.HasValue)
                {
                    throw new ValidationException("Cannot delete the root page directory.");
                }

                pageDirectory.IsActive = false;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageDirectoryEntityDefinition.DefinitionCode, pageDirectory.PageDirectoryId));

                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _cache.Clear();
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDirectoryCommand command)
        {
            yield return new PageDirectoryDeletePermission();
        }

        #endregion
    }
}
