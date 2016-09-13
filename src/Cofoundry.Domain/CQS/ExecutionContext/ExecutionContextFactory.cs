using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conditions;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Factory for creating an IExecutionContext instance.
    /// </summary>
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly IUserContextService _userContextService;

        public ExecutionContextFactory(
            IUserContextService userContextService
            )
        {
            _userContextService = userContextService;
        }

        public IExecutionContext Create()
        {
            return Create(_userContextService.GetCurrentContext());
        }

        public IExecutionContext Create(IUserContext userContext)
        {
            Condition.Requires(userContext).IsNotNull();

            var newContext = new ExecutionContext();
            newContext.ExecutionDate = DateTime.UtcNow;
            newContext.UserContext = userContext;

            return newContext;
        }
    }
}
