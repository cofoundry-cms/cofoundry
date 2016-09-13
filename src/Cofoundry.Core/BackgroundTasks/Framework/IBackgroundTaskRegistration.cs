using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.BackgroundTasks
{
    /// <summary>
    /// Implement this to allow automatic registration of background tasks
    /// during a boostrap process.
    /// </summary>
    public interface IBackgroundTaskRegistration
    {
        void Register(IBackgroundTaskScheduler scheduler);
    }
}
