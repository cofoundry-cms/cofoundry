using Cofoundry.Core.Reflection.Internal;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Default implementation of <see cref="IQueryExecutor"/>.
/// </summary>
/// <remarks>
/// See http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92
/// </remarks>
public class QueryExecutor : IQueryExecutor
{
    private static readonly MethodInfo _executeAsyncMethod = MethodReferenceHelper.GetPrivateInstanceMethod<QueryExecutor>(nameof(ExecuteQueryAsync));

    private readonly IModelValidationService _modelValidationService;
    private readonly IQueryHandlerFactory _queryHandlerFactory;
    private readonly IExecutionContextFactory _executionContextFactory;
    private readonly IExecutePermissionValidationService _executePermissionValidationService;

    public QueryExecutor(
        IModelValidationService modelValidationService,
        IQueryHandlerFactory queryHandlerFactory,
        IExecutionContextFactory executionContextFactory,
        IExecutePermissionValidationService executePermissionValidationService
        )
    {
        _modelValidationService = modelValidationService;
        _queryHandlerFactory = queryHandlerFactory;
        _executionContextFactory = executionContextFactory;
        _executePermissionValidationService = executePermissionValidationService;
    }

    /// <inheritdoc/>
    public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query)
    {
        IExecutionContext? executionContext = null;

        return ExecuteAsync(query, executionContext);
    }

    /// <inheritdoc/>
    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext? executionContext)
    {
        ArgumentNullException.ThrowIfNull(query);
        TResult result;

        try
        {
            var task = _executeAsyncMethod
                .MakeGenericMethod(query.GetType(), typeof(TResult))
                .Invoke(this, [query, executionContext]) as Task<TResult>;

            if (task == null)
            {
                throw new InvalidCastException($"Expected {_executeAsyncMethod.Name} to return a Task but found null.");
            }

            result = await task;
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException == null)
            {
                throw;
            }
            result = HandleException<TResult>(ex.InnerException);
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(userContext);

        var executionContext = _executionContextFactory.Create(userContext);
        return ExecuteAsync(query, executionContext);
    }

    private async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query, IExecutionContext? executionContext) where TQuery : IQuery<TResult>
    {
        var cx = await CreateExecutionContextAsync(executionContext);

        var handler = _queryHandlerFactory.CreateAsyncHandler<TQuery, TResult>();
        if (handler == null)
        {
            throw new MissingHandlerMappingException(typeof(TQuery));
        }

        _modelValidationService.Validate(query);
        _executePermissionValidationService.Validate(query, handler, cx);
        var result = await handler.ExecuteAsync(query, cx);

        return result;
    }

    private async Task<IExecutionContext> CreateExecutionContextAsync(IExecutionContext? cx)
    {
        if (cx == null)
        {
            return await _executionContextFactory.CreateAsync();
        }

        if (cx.UserContext == null)
        {
            throw new ExecutionContextNotInitializedException("The UserContext property cannot be null");
        }

        if (cx.ExecutionDate == DateTime.MinValue)
        {
            throw new ExecutionContextNotInitializedException("The ExecutionDate property has not been set");
        }

        return cx;
    }

    private static TResult HandleException<TResult>(Exception innerEx)
    {
        var info = ExceptionDispatchInfo.Capture(innerEx);
        info.Throw();

        // compiler requires assignment
        return default;
    }
}