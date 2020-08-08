using Cofoundry.Core.BackgroundTasks;
using Cofoundry.Domain.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.BackgroundTaskRegistrations
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
