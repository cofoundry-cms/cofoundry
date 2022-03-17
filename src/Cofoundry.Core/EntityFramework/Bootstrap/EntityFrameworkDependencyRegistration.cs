using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.EntityFramework.Internal;

namespace Cofoundry.Core.EntityFramework.Registration;

public class EntityFrameworkDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IEntityFrameworkSqlExecutor, EntityFrameworkSqlExecutor>()
            .Register<ISqlParameterFactory, SqlParameterFactory>()
            .Register<ICofoundryDbContextInitializer, CofoundryDbContextInitializer>()
            ;
    }
}
