using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain.CQS.Internal
{

    /// <summary>
    /// Service for executing commands of various types.
    /// </summary>
    /// <remarks>
    /// Inspiration taken from http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91,
    /// but has been adapted a fair bit.
    /// </remarks>
    public class CommandExecutor : ICommandExecutor
    {
        private static readonly MethodInfo _executeAsyncMethod = typeof(CommandExecutor).GetMethod(nameof(ExecuteCommandAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly ConcurrentDictionary<Type, MethodInfo> _cachedAsyncMethods = new ConcurrentDictionary<Type, MethodInfo>();

        #region constructor

        private readonly IModelValidationService _modelValidationService;
        private readonly DbContext _dbContext;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly ICommandLogService _commandLogService;
        private readonly IExecutePermissionValidationService _executePermissionValidationService;

        public CommandExecutor(
            DbContext dbContext,
            IModelValidationService modelValidationService,
            ICommandHandlerFactory commandHandlerFactory,
            IExecutionContextFactory executionContextFactory,
            ICommandLogService commandLogService,
            IExecutePermissionValidationService executePermissionValidationService
            )
        {
            _dbContext = dbContext;
            _modelValidationService = modelValidationService;
            _commandHandlerFactory = commandHandlerFactory;
            _executionContextFactory = executionContextFactory;
            _commandLogService = commandLogService;
            _executePermissionValidationService = executePermissionValidationService;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Creates an generic version of the execute method, but does
        /// some type inheritance lookup to make sure we are creating a handler
        /// for the command that actually implements the ICommand interface. Used
        /// in cases where a class inherits from a Command to add additional 
        /// information (e.g. in a ViewModel) but doesnt implement its own handler.
        /// </summary>
        private MethodInfo CreateExecuteMethod(MethodInfo executeMethod, Type type, Type previousType = null)
        {
            var isAssignable = typeof(ICommand).IsAssignableFrom(type);
            var typeInfo = type.GetTypeInfo();

            if (isAssignable && typeInfo.BaseType != null && !typeInfo.BaseType.GetTypeInfo().IsAbstract)
            {
                // If its valid, but the base type is too then search the basetype
                return CreateExecuteMethod(executeMethod, typeInfo.BaseType, type);
            }
            else if (isAssignable)
            {
                // If its assignable but the base type isnt valid
                return executeMethod.MakeGenericMethod(type);
            }
            else if (!isAssignable && previousType != null)
            {
                // If its not valid, then the previous type must be the implementer
                return executeMethod.MakeGenericMethod(previousType);
            }

            var msg = string.Format("Unexpected condition creating a generic CommandHandler for type '{0}'. Previous Type: '{1}'. IsAssignable: '{2}'", type, previousType, isAssignable);
            throw new InvalidOperationException(msg);
        }

        #region async execute

        /// <summary>
        /// Handles the execution the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        public async Task ExecuteAsync(ICommand command)
        {
            await ExecuteAsync(command, null);
        }

        /// <summary>
        /// Handles the execution the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext = null)
        {
            if (command == null) return;
            try
            {
                var commandType = command.GetType();
                var genericExecuteMethod = _cachedAsyncMethods.GetOrAdd(commandType, t => { return CreateExecuteMethod(_executeAsyncMethod, t); });
                await (Task)genericExecuteMethod.Invoke(this, new object[] { command, executionContext });
            }
            catch (TargetInvocationException ex)
            {
                HandleException(ex);
            }
        }

        private async Task ExecuteCommandAsync<TCommand>(TCommand command, IExecutionContext executionContext) where TCommand : ICommand
        {
            if (command == null) return;

            var cx = await CreateExecutionContextAsync(executionContext);
            var handler = _commandHandlerFactory.CreateAsyncHandler<TCommand>();
            if (handler == null)
            {
                throw new MissingHandlerMappingException(typeof(TCommand));
            }

            try
            {
                _modelValidationService.Validate(command);
                _executePermissionValidationService.Validate(command, handler, cx);
                await handler.ExecuteAsync(command, cx);
            }
            catch (Exception ex)
            {
                await _commandLogService.LogFailedAsync(command, cx, ex);
                throw;
            }

            await _commandLogService.LogAsync(command, cx);
        }

        #endregion

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

        private void HandleException(TargetInvocationException ex)
        {
            var info = ExceptionDispatchInfo.Capture(ex.InnerException);
            info.Throw();
        }

        #endregion
    }
}
