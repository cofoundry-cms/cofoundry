using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class ContentRepositoryDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container.Register<ContentRepository>([typeof(IContentRepository), typeof(IAdvancedContentRepository), typeof(IDomainRepository)]);
        container.Register<IDomainRepositoryExecutor, DomainRepositoryExecutor>();
        container.Register<IDomainRepositoryTransactionManager, DomainRepositoryTransactionManager>();
    }
}
