using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.AutoUpdate;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Installation
{
    /// <summary>
    /// Runs the RegisterDefinedRolesCommand at startup, adding new roles
    /// defined in code to the system, but leaving existing ones alone.
    /// </summary>
    public class RegisterNewDefinedRolesCommandHandler : IAsyncAlwaysRunUpdateCommandHandler<RegisterNewDefinedRolesCommand>
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IExecutionContextFactory _executionContextFactory;

        public RegisterNewDefinedRolesCommandHandler(
            ICommandExecutor commandExecutor,
            IExecutionContextFactory executionContextFactory
            )
        {
            _commandExecutor = commandExecutor;
            _executionContextFactory = executionContextFactory;
        }


        public async Task ExecuteAsync(RegisterNewDefinedRolesCommand command)
        {
            var cx = await _executionContextFactory.CreateSystemUserExecutionContextAsync();
            await _commandExecutor.ExecuteAsync(new RegisterDefinedRolesCommand(), cx);
        }
    }
}
