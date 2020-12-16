using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Handles the execution IQuery instances.
    /// </summary>
    /// <remarks>
    /// See http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92
    /// </remarks>
    public class QueryExecutor : IQueryExecutor
    {
        private static readonly MethodInfo _executeAsyncMethod = typeof(QueryExecutor).GetMethod(nameof(ExecuteQueryAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        
        #region constructor

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

        #endregion

        #region async execution

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query)
        {
            return await ExecuteAsync(query, null);
        }

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext = null)
        {
            TResult result;

            if (query == null) return default(TResult);
            try
            {
                result = await (Task<TResult>)_executeAsyncMethod
                    .MakeGenericMethod(query.GetType(), typeof(TResult))
                    .Invoke(this, new object[] { query, executionContext });
            }
            catch (TargetInvocationException ex)
            {
                result = HandleException<TResult>(ex);
            }

            return result;
        }

        private async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query, IExecutionContext executionContext) where TQuery : IQuery<TResult>
        {
            if (query == null) return default(TResult);

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

        #endregion

        #region helpers

        private async Task<IExecutionContext> CreateExecutionContextAsync(IExecutionContext cx)
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

        private TResult HandleException<TResult>(TargetInvocationException ex)
        {
            var info = ExceptionDispatchInfo.Capture(ex.InnerException);
            info.Throw();

            // compiler requires assignment
            return default(TResult);
        }

        #endregion
    }
}
