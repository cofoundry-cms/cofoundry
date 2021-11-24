using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
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
        private readonly IMessageAggregator _messageAggregator;

        public DeletePageDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageDirectoryCache cache,
            ITransactionScopeManager transactionScopeFactory,
            IPermissionValidationService permissionValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _cache = cache;
            _transactionScopeFactory = transactionScopeFactory;
            _permissionValidationService = permissionValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(DeletePageDirectoryCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .SingleOrDefaultAsync(d => d.PageDirectoryId == command.PageDirectoryId);

            if (pageDirectory == null) return;
            ValidateNotRootDirectory(pageDirectory);

            var directoriesToDelete = await GetDirectoryIdsToDeleteAsync(command);
            await ValidateDependencies(PageDirectoryEntityDefinition.DefinitionCode, directoriesToDelete.Keys, executionContext);

            var pagesToDelete = await GetPageIdsToDeleteAndValidatePermissionAsync(directoriesToDelete.Keys, executionContext);
            await ValidateDependencies(PageEntityDefinition.DefinitionCode, pagesToDelete.Keys, executionContext);

            _dbContext.PageDirectories.Remove(pageDirectory);
            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(directoriesToDelete.Values, pagesToDelete.Values));
        }

        private async Task OnTransactionComplete(IReadOnlyCollection<PageDirectoryDeletedMessage> deletedDirectoryMessages, IReadOnlyCollection<PageDeletedMessage> deletedPageMessages)
        {
            _cache.Clear();

            await _messageAggregator.PublishBatchAsync(deletedDirectoryMessages);
            await _messageAggregator.PublishBatchAsync(deletedPageMessages);
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
        /// but we need the list to check constraints and publish messages.
        /// </summary>
        private async Task<Dictionary<int, PageDirectoryDeletedMessage>> GetDirectoryIdsToDeleteAsync(DeletePageDirectoryCommand command)
        {
            return await _dbContext
                .PageDirectoryClosures
                .AsNoTracking()
                .FilterByAncestorId(command.PageDirectoryId)
                .Select(d => new PageDirectoryDeletedMessage()
                {
                    PageDirectoryId = d.DescendantPageDirectoryId,
                    FullUrlPath = "/" + d.DescendantPageDirectory.PageDirectoryPath.FullUrlPath
                })
                .ToDictionaryAsync(k => k.PageDirectoryId);
        }

        /// <summary>
        /// Gets all pages that will be deleted when each of the specified directories
        /// are deleted. Note that these will be actually deleted via the database trigger
        /// but we need the list to check for constraints and publish messages.
        /// </summary>
        private async Task<Dictionary<int, PageDeletedMessage>> GetPageIdsToDeleteAndValidatePermissionAsync(ICollection<int> directoryIdsToDelete, IExecutionContext executionContext)
        {
            var pageIds = await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => directoryIdsToDelete.Contains(p.PageDirectoryId))
                .Select(p => p.PageId)
                .ToListAsync();

            if (pageIds.Any())
            {
                _permissionValidationService.EnforcePermission<PageDeletePermission>(executionContext.UserContext);
            }

            var pagesToDelete = await _queryExecutor.ExecuteAsync(new GetPageRoutesByIdRangeQuery(pageIds), executionContext);
            var results = new Dictionary<int, PageDeletedMessage>(pagesToDelete.Count);

            foreach (var pageToDelete in pagesToDelete.Values)
            {
                var result = new PageDeletedMessage()
                {
                    PageId = pageToDelete.PageId,
                    FullUrlPath = pageToDelete.FullUrlPath
                };

                results.Add(result.PageId, result);
            }

            return results;
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
