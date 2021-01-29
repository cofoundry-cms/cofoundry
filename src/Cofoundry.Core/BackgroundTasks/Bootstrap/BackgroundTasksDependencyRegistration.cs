using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.BackgroundTasks.Registration
{
    public class BackgroundTasksDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterAll<IRecurringBackgroundTask>();
            container.RegisterAll<IAsyncRecurringBackgroundTask>();
            container.RegisterAll<IBackgroundTaskRegistration>();
        }
    }
}
