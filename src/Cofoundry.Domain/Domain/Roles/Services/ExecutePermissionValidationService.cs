using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Service to validate the permissions of command/query handler prior to execution.
    /// </summary>
    public class ExecutePermissionValidationService : IExecutePermissionValidationService
    {
        #region constructor

        private readonly IPermissionValidationService _permissionValidationService;

        public ExecutePermissionValidationService(
            IPermissionValidationService permissionValidationService
            )
        {
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        public virtual void Validate<TCommand>(TCommand command, IAsyncCommandHandler<TCommand> commandHandler, IExecutionContext executionContext) where TCommand : ICommand
        {
            ValidateCommmandImplementation<TCommand>(commandHandler);

            if (commandHandler is IPermissionRestrictedCommandHandler<TCommand>)
            {
                var permissions = ((IPermissionRestrictedCommandHandler<TCommand>)commandHandler).GetPermissions(command);
                _permissionValidationService.EnforcePermission(permissions, executionContext.UserContext);
            }

            CheckAdditionalPermissionHandlers(commandHandler, executionContext, _permissionValidationService);
        }

        public virtual void Validate<TQuery, TResult>(TQuery query, IAsyncQueryHandler<TQuery, TResult> queryHandler, IExecutionContext executionContext) where TQuery : IQuery<TResult>
        {
            ValidateQueryImplementation<TQuery, TResult>(queryHandler);

            if (queryHandler is IPermissionRestrictedQueryHandler<TQuery, TResult>)
            {
                var permissions = ((IPermissionRestrictedQueryHandler<TQuery, TResult>)queryHandler).GetPermissions(query);
                _permissionValidationService.EnforcePermission(permissions, executionContext.UserContext);
            }

            CheckAdditionalPermissionHandlers(queryHandler, executionContext, _permissionValidationService);
        }

        #region private

        protected void ValidateCommmandImplementation<TCommand>(object handler)
            where TCommand : ICommand
        {
            if (handler is IPermissionRestrictedCommandHandler)
            {
                // Check for invalid implementation
                if (!(handler is IPermissionRestrictedCommandHandler<TCommand>))
                {
                    var msg = string.Format("Invalid implementation: {0} imeplements IPermissionRestrictedCommandHandler but not IPermissionRestrictedCommandHandler<{1}>", handler.GetType().FullName, typeof(TCommand).FullName);
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                ValidateIgnorePermissionImplementation(handler);
            }
        }

        protected void ValidateQueryImplementation<TQuery, TResult>(object handler)
            where TQuery : IQuery<TResult>
        {
            if (handler is IPermissionRestrictedQueryHandler)
            {
                // Check for invalid implementation
                if (!(handler is IPermissionRestrictedQueryHandler<TQuery, TResult>))
                {
                    var msg = string.Format("Invalid implementation: {0} imeplements IPermissionRestrictedCommandHandler but not IPermissionRestrictedCommandHandler<{1}>", handler.GetType().FullName, typeof(TQuery).FullName);
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                ValidateIgnorePermissionImplementation(handler);
            }
        }

        protected void CheckAdditionalPermissionHandlers<TCommandHandler>(TCommandHandler _commandHandler, IExecutionContext executionContext, IPermissionValidationService _permissionValidationService)
        {
            // Hardcoded checking of a few additional handlers, but could use DI here to make this more flexible.
            if (_commandHandler is ILoggedInPermissionCheckHandler)
            {
                _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);
            }

            if (_commandHandler is ICofoundryUserPermissionCheckHandler)
            {
                _permissionValidationService.EnforceHasPermissionToUserArea(CofoundryAdminUserArea.AreaCode, executionContext.UserContext);
            }
        }

        /// <summary>
        /// Here we check that IIgnorePermissionCheckHandler has been implemented if no other
        /// permission handling is implemented, this ensures that the implementer has thought 
        /// about permission handling and hasn't just forgotton to add it.
        /// </summary>
        protected void ValidateIgnorePermissionImplementation(object handler)
        {
            // HACK: Here we have exclude handlers in Cofoundry.Core because they cannot implement the 
            // Cofoundry permission interfaces. This is a bit hacky but I can't think of another easy way
            // to do it and this should be fine for the forseeable future.
            if (!(handler is IPermissionCheckHandler) && !handler.GetType().Namespace.StartsWith("Cofoundry.Core"))
            {
                var msg = string.Format("{0} does not implement an IPermissionCheckHandler. Either implement one of the permission checking interfaces or use IIgnorePermissionCheckHandler if no permission handling is required.", handler.GetType().FullName);
                throw new InvalidOperationException(msg);
            }
        }

        #endregion
    }
}
