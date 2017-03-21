using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class DeleteWebDirectoryCommandHandler 
        : IAsyncCommandHandler<DeleteWebDirectoryCommand>
        , IPermissionRestrictedCommandHandler<DeleteWebDirectoryCommand>
    {
        #region constructor 

        private readonly CofoundryDbContext _dbContext;
        private readonly IWebDirectoryCache _cache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly ICommandExecutor _commandExecutor;

        public DeleteWebDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IWebDirectoryCache cache,
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

        public async Task ExecuteAsync(DeleteWebDirectoryCommand command, IExecutionContext executionContext)
        {
            var webDirectory = await _dbContext
                .WebDirectories
                .Include(w => w.ChildWebDirectories)
                .SingleOrDefaultAsync(w => w.WebDirectoryId == command.WebDirectoryId);

            if (webDirectory != null)
            {
                if (!webDirectory.ParentWebDirectoryId.HasValue)
                {
                    throw new ValidationException("Cannot delete the root web directory.");
                }

                webDirectory.IsActive = false;

                using (var scope = _transactionScopeFactory.Create())
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(WebDirectoryEntityDefinition.DefinitionCode, webDirectory.WebDirectoryId));

                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _cache.Clear();
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteWebDirectoryCommand command)
        {
            yield return new WebDirectoryDeletePermission();
        }

        #endregion
    }
}
