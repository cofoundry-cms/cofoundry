namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Service to validate the permissions of a command or query handler prior to execution.
    /// </summary>
    public interface IExecutePermissionValidationService
    {
        void Validate<TCommand>(TCommand command, ICommandHandler<TCommand> commandHandler, IExecutionContext executionContext) where TCommand : ICommand;

        void Validate<TQuery, TResult>(TQuery query, IQueryHandler<TQuery, TResult> queryHandler, IExecutionContext executionContext) where TQuery : IQuery<TResult>;
    }
}
