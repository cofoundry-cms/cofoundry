using Cofoundry.Core.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class AssetFileCleanupBackgroundTaskRegistration : IBackgroundTaskRegistration
    {
        public void Register(IBackgroundTaskScheduler scheduler)
        {
            scheduler.RegisterAsyncRecurringTask<AssetFileCleanupBackgroundTask>(60);
        }
    }
}
