using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Shared.Mocks
{
    public class MockCommandHandler<TCommand>
        : ICommandHandler<TCommand>
        , IIgnorePermissionCheckHandler
        where TCommand : ICommand
    {
        private readonly Func<TCommand, Task> _handlerDelegate;

        public MockCommandHandler(Action<TCommand> handlerDelegate)
        {
            _handlerDelegate = command =>
            {
                handlerDelegate(command);
                return Task.CompletedTask;
            };
        }

        public MockCommandHandler(Func<TCommand, Task> asyncHandlerDelegate)
        {
            _handlerDelegate = asyncHandlerDelegate;
        }

        public Task ExecuteAsync(TCommand command, IExecutionContext executionContext)
        {
            return _handlerDelegate.Invoke(command);
        }
    }
}