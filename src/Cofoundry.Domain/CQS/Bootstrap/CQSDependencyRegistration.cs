using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.CQS.Internal;

namespace Cofoundry.Domain.CQS.Registration;

public class CQSDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<ICommandExecutor, CommandExecutor>()
            .RegisterAllGenericImplementations(typeof(ICommandHandler<>))
            .Register<ICommandHandlerFactory, CommandHandlerFactory>()
            .Register<IQueryExecutor, QueryExecutor>()
            .RegisterAllGenericImplementations(typeof(IQueryHandler<,>))
            .Register<IQueryHandlerFactory, QueryHandlerFactory>()
            .Register<ICommandLogService, DebugCommandLogService>()
            .Register<IExecutionContextFactory, ExecutionContextFactory>()
            ;
    }
}
