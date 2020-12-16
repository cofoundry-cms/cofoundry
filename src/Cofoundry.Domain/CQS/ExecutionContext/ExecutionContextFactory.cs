using Cofoundry.Core.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Factory for creating an IExecutionContext instance.
    /// </summary>
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        #region constructor

        private readonly IUserContextService _userContextService;
        private readonly IDateTimeService _dateTimeService;

        public ExecutionContextFactory(
            IUserContextService userContextService,
            IDateTimeService dateTimeService
            )
        {
            _userContextService = userContextService;
            _dateTimeService = dateTimeService;
        }

        #endregion

        /// <summary>
        /// Creates an instance of IExecutionContext from the currently
        /// logged in user.
        /// </summary>
        public async Task<IExecutionContext> CreateAsync()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            return Create(userContext);
        }

        /// <summary>
        /// Creates an instance of IExecutionContext from the
        /// specified IUserContext. Can be used to impersonate another user.
        /// </summary>
        /// <param name="userContext">IUserContext to impersonate</param>
        /// <param name="executionContextToCopy">Optional execution context to base the new context on</param>
        public IExecutionContext Create(IUserContext userContext, IExecutionContext executionContextToCopy = null)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            var newContext = new ExecutionContext();
            newContext.UserContext = userContext;

            if (executionContextToCopy != null)
            {
                newContext.ExecutionDate = executionContextToCopy.ExecutionDate;
            }
            else
            {
                newContext.ExecutionDate = _dateTimeService.UtcNow();
            }

            return newContext;
        }

        /// <summary>
        /// Creates an instance of IExecutionContext using the system account. Should 
        /// be used sparingly for elevating permissions, typically for back-end processes.
        /// </summary>
        /// <param name="executionContextToCopy">Optional execution context to base the new context on</param>
        public async Task<IExecutionContext> CreateSystemUserExecutionContextAsync(IExecutionContext executionContextToCopy = null)
        {
            var userContext = await _userContextService.GetSystemUserContextAsync();
            return Create(userContext, executionContextToCopy);
        }
    }
}
