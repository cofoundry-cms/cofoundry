using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.CQS
{
    public class CQSDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<ICommandExecutor, CommandExecutor>()
                .RegisterAllGenericImplementations(typeof(IAsyncCommandHandler<>))
                .RegisterType<ICommandHandlerFactory, CommandHandlerFactory>()
                .RegisterType<IQueryExecutor, QueryExecutor>()
                .RegisterAllGenericImplementations(typeof(IAsyncQueryHandler<,>))
                .RegisterType<IQueryHandlerFactory, QueryHandlerFactory>()
                .RegisterType<ICommandLogService, DebugCommandLogService>()
                .RegisterType<IExecutionContextFactory, ExecutionContextFactory>()
                ; 
        }
    }
}
