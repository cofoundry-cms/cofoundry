using Cofoundry.Core.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class AssetFileCleanupBackgroundTaskRegistration : IBackgroundTaskRegistration
    {
        private readonly AssetFileCleanupSettings _assetFileCleanupSettings;


        public AssetFileCleanupBackgroundTaskRegistration(
            AssetFileCleanupSettings assetFileCleanupSettings
            )
        {
            _assetFileCleanupSettings = assetFileCleanupSettings;
        }
        public void Register(IBackgroundTaskScheduler scheduler)
        {
            scheduler.RegisterAsyncRecurringTask<AssetFileCleanupBackgroundTask>(_assetFileCleanupSettings.BackgroundTaskFrequencyInMinutes);
        }
    }
}
