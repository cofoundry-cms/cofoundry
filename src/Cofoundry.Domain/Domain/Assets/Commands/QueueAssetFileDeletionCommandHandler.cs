using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds an asset file reference to a queue so that it
    /// can be deleted by a background process.
    /// </summary>
    public class QueueAssetFileDeletionCommandHandler 
        : ICommandHandler<QueueAssetFileDeletionCommand>
        , IPermissionRestrictedCommandHandler<QueueAssetFileDeletionCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public QueueAssetFileDeletionCommandHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(QueueAssetFileDeletionCommand command, IExecutionContext executionContext)
        {
            var queueItem = new AssetFileCleanupQueueItem()
            {
                EntityDefinitionCode = command.EntityDefinitionCode,
                FileNameOnDisk = command.FileNameOnDisk,
                FileExtension = command.FileExtension,
                CanRetry = true,
                AttemptPermittedDate = executionContext.ExecutionDate,
                CreateDate = executionContext.ExecutionDate
            };

            _dbContext.AssetFileCleanupQueueItems.Add(queueItem);
            await _dbContext.SaveChangesAsync();
        }

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(QueueAssetFileDeletionCommand command)
        {
            yield return new ImageAssetDeletePermission();
            yield return new DocumentAssetDeletePermission();
        }

        #endregion
    }
}
