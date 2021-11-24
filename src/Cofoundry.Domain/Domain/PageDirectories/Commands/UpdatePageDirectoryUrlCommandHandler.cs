using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the url of a page directory. Changing a directory url
    /// will cause the url of any child directories or pages to change. The command
    /// will publish an <see cref="PageDirectoryUrlChangedMessage"/> or <see cref="PageUrlChangedMessage"/>
    /// for any affected directories or pages.
    /// </summary>
    public class UpdatePageDirectoryUrlCommandHandler
        : ICommandHandler<UpdatePageDirectoryUrlCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageDirectoryUrlCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageDirectoryStoredProcedures _pageDirectoryStoredProcedures;
        private readonly IPageDirectoryCache _cache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPermissionValidationService _permissionValidationService;

        public UpdatePageDirectoryUrlCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageDirectoryStoredProcedures pageDirectoryStoredProcedures,
            IPageDirectoryCache cache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageDirectoryStoredProcedures = pageDirectoryStoredProcedures;
            _cache = cache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
            _permissionValidationService = permissionValidationService;
        }

        public async Task ExecuteAsync(UpdatePageDirectoryUrlCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .FilterById(command.PageDirectoryId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(pageDirectory, command.PageDirectoryId);

            var changedPathProps = GetChangedPathProperties(command, pageDirectory).ToArray();
            if (!changedPathProps.Any()) return;

            await ValidateIsUniqueAsync(command, executionContext);
            var affectedDirectories = await GetAffectedDirectoriesAsync(command);
            ValidateParentDirectory(command, affectedDirectories.Keys);
            var affectedPages = await GetAffectedPagesAndValidatePermissionsAsync(affectedDirectories.Keys, executionContext);

            pageDirectory.UrlPath = command.UrlPath;
            pageDirectory.ParentPageDirectoryId = command.ParentPageDirectoryId;

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                if (changedPathProps.Contains(nameof(command.ParentPageDirectoryId)))
                {
                    await _pageDirectoryStoredProcedures.UpdatePageDirectoryClosureAsync();
                }
                else if (changedPathProps.Contains(nameof(command.UrlPath)))
                {
                    await _pageDirectoryStoredProcedures.UpdatePageDirectoryPathAsync();
                }

                scope.QueueCompletionTask(() => OnTransactionComplete(affectedDirectories.Values, affectedPages));
                await scope.CompleteAsync();
            }
        }

        private void ValidateParentDirectory(UpdatePageDirectoryUrlCommand command, ICollection<int> affectedDirectoryIds)
        {
            if (affectedDirectoryIds.Contains(command.ParentPageDirectoryId))
            {
                throw ValidationErrorException.CreateWithProperties("The parent directory cannot be a child of this directory.", nameof(command.ParentPageDirectoryId));
            }
        }

        private async Task OnTransactionComplete(IReadOnlyCollection<PageDirectoryUrlChangedMessage> directoryMessages, IReadOnlyCollection<PageUrlChangedMessage> pageMessages)
        {
            _cache.Clear();

            await _messageAggregator.PublishBatchAsync(directoryMessages);
            await _messageAggregator.PublishBatchAsync(pageMessages);
        }

        private IEnumerable<string> GetChangedPathProperties(UpdatePageDirectoryUrlCommand command, PageDirectory pageDirectory)
        {
            if (command.UrlPath != pageDirectory.UrlPath) yield return nameof(pageDirectory.UrlPath);
            if (command.ParentPageDirectoryId != pageDirectory.ParentPageDirectoryId) yield return nameof(pageDirectory.ParentPageDirectoryId);
        }

        private async Task ValidateIsUniqueAsync(UpdatePageDirectoryUrlCommand command, IExecutionContext executionContext)
        {
            var query = new IsPageDirectoryPathUniqueQuery();
            query.ParentPageDirectoryId = command.ParentPageDirectoryId;
            query.UrlPath = command.UrlPath;
            query.PageDirectoryId = command.PageDirectoryId;

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                var message = $"A page directory already exists in that directory with the path '{command.UrlPath}'";
                throw new UniqueConstraintViolationException(message, nameof(command.UrlPath), command.UrlPath);
            }
        }

        private async Task<Dictionary<int, PageDirectoryUrlChangedMessage>> GetAffectedDirectoriesAsync(UpdatePageDirectoryUrlCommand command)
        {
            return await _dbContext
                .PageDirectoryClosures
                .AsNoTracking()
                .FilterByAncestorId(command.PageDirectoryId)
                .Select(d => new PageDirectoryUrlChangedMessage()
                {
                    PageDirectoryId = d.DescendantPageDirectoryId,
                    OldFullUrlPath = "/" + d.DescendantPageDirectory.PageDirectoryPath.FullUrlPath
                })
                .ToDictionaryAsync(k => k.PageDirectoryId);
        }

        private async Task<List<PageUrlChangedMessage>> GetAffectedPagesAndValidatePermissionsAsync(ICollection<int> affectedPageDirectoryIds, IExecutionContext executionContext)
        {
            var pageIds = await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => affectedPageDirectoryIds.Contains(p.PageDirectoryId))
                .Select(p => p.PageId)
                .ToListAsync();

            if (pageIds.Any())
            {
                _permissionValidationService.EnforcePermission<PageUpdateUrlPermission>(executionContext.UserContext);
            }

            var affectedPages = await _queryExecutor.ExecuteAsync(new GetPageRoutesByIdRangeQuery(pageIds), executionContext);
            var results = new List<PageUrlChangedMessage>(affectedPages.Count);

            foreach (var affectedPage in affectedPages.Values)
            {
                var result = new PageUrlChangedMessage()
                {
                    PageId = affectedPage.PageId,
                    OldFullUrlPath = affectedPage.FullPath,
                    HasPublishedVersionChanged = affectedPage.PublishStatus == PublishStatus.Published
                };

                results.Add(result);
            }

            return results;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryUrlCommand command)
        {
            yield return new PageDirectoryUpdateUrlPermission();
        }
    }
}
