using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Core.ExecutionDurationRandomizer.Internal;

namespace Cofoundry.Core.Registration
{
    public class ExecutionDurationRandomizerRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.RegisterScoped<IExecutionDurationRandomizerScopeManager, ExecutionDurationRandomizerScopeManager>();
        }
    }
}
