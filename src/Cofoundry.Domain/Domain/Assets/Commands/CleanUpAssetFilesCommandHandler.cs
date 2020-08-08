using Cofoundry.Core.DistributedLocks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// <para>
    /// Processes a batch of items in the asset file cleanup 
    /// queue, deleting any asset files associated with the queue
    /// items. If any errors occur then the item is re-queued to
    /// try again later.
    /// </para>
    /// <para>
    /// The process is run inside a distributed lock to prevent
    /// the process running concurrently.
    /// </para>
    /// </summary>
    public class CleanUpAssetFilesCommandHandler
        : ICommandHandler<CleanUpAssetFilesCommand>
        , IPermissionRestrictedCommandHandler<CleanUpAssetFilesCommand>
    {
        private readonly IDistributedLockManager _distributedLockManager;
        private readonly CofoundryDbContext _dbContext;
        private readonly IFileStoreService _fileStoreService;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;
        private readonly AssetFileCleanupSettings _assetFileCleanupSettings;
        private readonly ILogger<CleanUpAssetFilesCommandHandler> _logger;

        public CleanUpAssetFilesCommandHandler(
            IDistributedLockManager distributedLockManager,
            CofoundryDbContext dbContext,
            IFileStoreService fileStoreService,
            IResizedImageAssetFileService resizedImageAssetFileService,
            AssetFileCleanupSettings assetFileCleanupSettings,
            ILogger<CleanUpAssetFilesCommandHandler> logger
            )
        {
            _distributedLockManager = distributedLockManager;
            _dbContext = dbContext;
            _fileStoreService = fileStoreService;
            _resizedImageAssetFileService = resizedImageAssetFileService;
            _assetFileCleanupSettings = assetFileCleanupSettings;
            _logger = logger;
        }

        public async Task ExecuteAsync(CleanUpAssetFilesCommand command, IExecutionContext executionContext)
        {
            // We use the distributed lock manager to make sure we don't have two
            // processes running at the same time trying to clean up the same files.
            var distributedLock = await _distributedLockManager.LockAsync<CleanUpAssetFilesCommandDistributedLockDefinition>();

            if (distributedLock.IsLockedByAnotherProcess())
            {
                _logger.LogInformation($"{nameof(CleanUpAssetFilesCommandHandler)} requested to be run, but is already locked by another process.");

                return;
            }

            try
            {
                var queueItems = await GetQueueBatch(command.BatchSize, executionContext);

                if (queueItems.Any())
                {
                    await ProcessQueueAsync(queueItems);
                }
            }
            finally
            {
                await _distributedLockManager.UnlockAsync(distributedLock);
            }
        }

        private async Task ProcessQueueAsync(List<AssetFileCleanupQueueItem> queueItems)
        {
            _logger.LogInformation("Processing {NumItems} items from the queue.", queueItems.Count);

            foreach (var item in queueItems)
            {
                var fileName = Path.ChangeExtension(item.FileNameOnDisk, item.FileExtension);
                item.LastAttemptDate = DateTime.UtcNow;

                try
                {
                    // This could be pluggable in the future if there was a need
                    switch (item.EntityDefinitionCode)
                    {
                        case ImageAssetEntityDefinition.DefinitionCode:
                            await _fileStoreService.DeleteAsync(ImageAssetConstants.FileContainerName, fileName);
                            await _resizedImageAssetFileService.ClearAsync(item.FileNameOnDisk);
                            await MarkCompleteAsync(item);
                            break;
                        case DocumentAssetEntityDefinition.DefinitionCode:
                            await _fileStoreService.DeleteAsync(DocumentAssetConstants.FileContainerName, fileName);
                            await MarkCompleteAsync(item);
                            break;
                        default:
                            _logger.LogError("Invalid asset EntityDefinitionCode for queue item {AssetFileCleanupQueueItemId}: {EntityDefinitionCode}", item.AssetFileCleanupQueueItemId, item.EntityDefinitionCode);
                            await MarkCannotRetryAsync(item);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning asset file queue item {AssetFileCleanupQueueItemId} of type {EntityDefinitionCode} and filename: {FileName}", item.AssetFileCleanupQueueItemId, item.EntityDefinitionCode, fileName);
                    await MarkFailedAsync(item);
                }
            }
        }

        private Task MarkCompleteAsync(AssetFileCleanupQueueItem item)
        {
            item.CompletedDate = DateTime.UtcNow;
            return _dbContext.SaveChangesAsync();
        }

        private async Task MarkFailedAsync(AssetFileCleanupQueueItem item)
        {
            var retryTimeDifference = item.AttemptPermittedDate - item.CreateDate;
            if (retryTimeDifference.Days > _assetFileCleanupSettings.MaxRetryWindowInDays)
            {
                await MarkCannotRetryAsync(item);
                return;
            }

            // for a first attempt the time difference will be 0 so we'll need to set an initial value.
            if (retryTimeDifference.TotalMinutes < _assetFileCleanupSettings.RetryOffsetInMinutes)
            {
                retryTimeDifference = TimeSpan.FromMinutes(_assetFileCleanupSettings.RetryOffsetInMinutes);
            }
            else
            {
                retryTimeDifference = TimeSpan.FromMinutes(retryTimeDifference.TotalMinutes * _assetFileCleanupSettings.RetryOffsetMultiplier);
            }
            item.AttemptPermittedDate = item.AttemptPermittedDate.Add(retryTimeDifference);

            await  _dbContext.SaveChangesAsync();
        }

        private Task MarkCannotRetryAsync(AssetFileCleanupQueueItem item)
        {
            item.CanRetry = false;
            return _dbContext.SaveChangesAsync();
        }

        private Task<List<AssetFileCleanupQueueItem>> GetQueueBatch(int batchSize, IExecutionContext executionContext)
        {
            return _dbContext
                .AssetFileCleanupQueueItems
                .Where(i => !i.CompletedDate.HasValue && i.CanRetry && i.AttemptPermittedDate < executionContext.ExecutionDate)
                .OrderBy(i => i.AttemptPermittedDate)
                .Take(batchSize)
                .ToListAsync();
        }

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(CleanUpAssetFilesCommand command)
        {
            yield return new ImageAssetDeletePermission();
            yield return new DocumentAssetDeletePermission();
        }

        #endregion
    }
}
