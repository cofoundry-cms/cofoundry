using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    public class UpdatePageDirectoryCommandHandler
        : ICommandHandler<UpdatePageDirectoryCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageDirectoryCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageDirectoryCache _cache;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageDirectoryCommandHandler(
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

        public async Task ExecuteAsync(UpdatePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .SingleOrDefaultAsync(w => w.PageDirectoryId == command.PageDirectoryId);
            EntityNotFoundException.ThrowIfNull(pageDirectory, command.PageDirectoryId);

            // Custom Validation
            await ValidateIsUniqueAsync(command, executionContext);
            await ValidateUrlPropertiesAllowedToChange(command, pageDirectory);

            pageDirectory.Name = command.Name;
            pageDirectory.UrlPath = command.UrlPath;
            pageDirectory.ParentPageDirectoryId = command.ParentPageDirectoryId;

            await _dbContext.SaveChangesAsync();

            _transactionScopeFactory.QueueCompletionTask(_dbContext, _cache.Clear);
        }

        private async Task ValidateUrlPropertiesAllowedToChange(UpdatePageDirectoryCommand command, PageDirectory pageDirectory)
        {
            var props = GetChangedPathProperties(command, pageDirectory);

            if (props.Any() && await HasDependencies(pageDirectory)) 
            {
                throw ValidationErrorException.CreateWithProperties("This directory is in use and the url cannot be changed", props.First());
            }
        }

        private IEnumerable<string> GetChangedPathProperties(UpdatePageDirectoryCommand command, PageDirectory pageDirectory) 
        {
            if (command.UrlPath != pageDirectory.UrlPath) yield return nameof(pageDirectory.UrlPath);
            if (command.ParentPageDirectoryId != pageDirectory.ParentPageDirectoryId) yield return nameof(pageDirectory.ParentPageDirectoryId);
        }

        private async Task<bool> HasDependencies(PageDirectory pageDirectory)
        {
            if (await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(w => w.ParentPageDirectoryId == pageDirectory.PageDirectoryId)
                .AnyAsync())
            {
                return true;
            }

            return await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => p.PageDirectoryId == pageDirectory.PageDirectoryId)
                .AnyAsync();
        }

        private async Task ValidateIsUniqueAsync(UpdatePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var query = new IsPageDirectoryPathUniqueQuery();
            query.ParentPageDirectoryId = command.ParentPageDirectoryId;
            query.UrlPath = command.UrlPath;
            query.PageDirectoryId = command.PageDirectoryId;

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                var message = $"A page directory already exists in that parent directory with the path '{command.UrlPath}'";
                throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryCommand command)
        {
            yield return new PageDirectoryUpdatePermission();
        }

        #endregion
    }
}
