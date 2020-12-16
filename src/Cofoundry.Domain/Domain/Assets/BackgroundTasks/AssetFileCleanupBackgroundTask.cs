using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.BackgroundTasks
{
    public class AssetFileCleanupBackgroundTask : IAsyncRecurringBackgroundTask
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly AssetFileCleanupSettings _assetFileCleanupSettings;

        public AssetFileCleanupBackgroundTask(
            ICommandExecutor commandExecutor,
            IExecutionContextFactory executionContextFactory,
            AssetFileCleanupSettings assetFileCleanupSettings
            )
        {
            _commandExecutor = commandExecutor;
            _executionContextFactory = executionContextFactory;
            _assetFileCleanupSettings = assetFileCleanupSettings;
        }

        public async Task ExecuteAsync()
        {
            if (_assetFileCleanupSettings.Disabled) return;

            var executionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync();

            var deleteFilesCommand = new CleanUpAssetFilesCommand()
            {
                BatchSize = _assetFileCleanupSettings.BatchSize
            };
            await _commandExecutor.ExecuteAsync(deleteFilesCommand, executionContext);

            var cleanUpCommand = new CleanupAssetFileCleanupQueueCommand()
            {
                CompletedItemRetentionTime = TimeSpan.FromMinutes(_assetFileCleanupSettings.CompletedItemRetentionTimeInMinutes),
                DeadLetterRetentionTime = TimeSpan.FromMinutes(_assetFileCleanupSettings.DeadLetterRetentionTimeInMinutes)
            };

            await _commandExecutor.ExecuteAsync(cleanUpCommand, executionContext);
        }
    }
}
