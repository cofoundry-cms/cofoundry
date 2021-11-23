using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class DeletePageDirectoryCommandHandler
        : ICommandHandler<DeletePageDirectoryCommand>
        , IPermissionRestrictedCommandHandler<DeletePageDirectoryCommand>
    {
        private static PageDirectoryEntityDefinition DIRECTORY_ENTITY_DEFINITION = new PageDirectoryEntityDefinition();

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageDirectoryCache _cache;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPermissionValidationService _permissionValidationService;

        public DeletePageDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageDirectoryCache cache,
            ITransactionScopeManager transactionScopeFactory,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _cache = cache;
            _transactionScopeFactory = transactionScopeFactory;
            _permissionValidationService = permissionValidationService;
        }

        public async Task ExecuteAsync(DeletePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .SingleOrDefaultAsync(d => d.PageDirectoryId == command.PageDirectoryId);

            if (pageDirectory == null) return;
            ValidateNotRootDirectory(pageDirectory);

            var directoryIdsToDelete = await GetDirectoryIdsToDelete(command);
            await ValidateDependencies(PageDirectoryEntityDefinition.DefinitionCode, directoryIdsToDelete, executionContext);

            var pageIdsToDelete = await GetPageIdsToDeleteAndValidatePermission(directoryIdsToDelete, executionContext);
            await ValidateDependencies(PageEntityDefinition.DefinitionCode, pageIdsToDelete, executionContext);

            _dbContext.PageDirectories.Remove(pageDirectory);
            await _dbContext.SaveChangesAsync();
            _transactionScopeFactory.QueueCompletionTask(_dbContext, () => _cache.Clear());
        }

        private static void ValidateNotRootDirectory(PageDirectory pageDirectory)
        {
            if (!pageDirectory.ParentPageDirectoryId.HasValue)
            {
                throw ValidationErrorException.CreateWithProperties("Cannot delete the root page directory.", nameof(pageDirectory.PageDirectoryId));
            }
        }


        /// <summary>
        /// Gets the ids of all directories that will be deleted including
        /// the directory specified in the command as well as any child directories.
        /// Note that related directories will actually be deleted by the database trigger
        /// but we need the list to check constraints etc.
        /// </summary>
        private async Task<List<int>> GetDirectoryIdsToDelete(DeletePageDirectoryCommand command)
        {
            return await _dbContext
                .PageDirectoryClosures
                .AsNoTracking()
                .FilterByAncestorId(command.PageDirectoryId)
                .Select(d => d.DescendantPageDirectoryId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all pages that will be deleted when each of the specified directories
        /// are deleted. Note that these will be actually deleted via the database trigger
        /// but we need the list to check for constraints etc.
        /// </summary>
        private async Task<ICollection<int>> GetPageIdsToDeleteAndValidatePermission(ICollection<int> directoryIdsToDelete, IExecutionContext executionContext)
        {
            var pageIdsToDelete = await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => directoryIdsToDelete.Contains(p.PageDirectoryId))
                .Select(p => p.PageId)
                .ToListAsync();

            if (pageIdsToDelete.Any())
            {
                _permissionValidationService.EnforcePermission<PageDeletePermission>(executionContext.UserContext);
            }

            return pageIdsToDelete;
        }

        private async Task ValidateDependencies(string entityDefinitionCode, ICollection<int> entityIds, IExecutionContext executionContext)
        {
            if (!entityIds.Any()) return;

            var requiredDependencies = await _queryExecutor.ExecuteAsync(new GetEntityDependencySummaryByRelatedEntityIdRangeQuery()
            {
                EntityDefinitionCode = entityDefinitionCode,
                EntityIds = entityIds,
                ExcludeDeletable = true
            }, executionContext);

            RequiredDependencyConstaintViolationException.ThrowIfCannotDelete(DIRECTORY_ENTITY_DEFINITION, requiredDependencies);
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDirectoryCommand command)
        {
            yield return new PageDirectoryDeletePermission();
        }
    }
}
