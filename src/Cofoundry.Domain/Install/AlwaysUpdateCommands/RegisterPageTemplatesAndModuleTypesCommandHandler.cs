using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.AutoUpdate;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Installation
{
    public class RegisterPageTemplatesAndModuleTypesCommandHandler : IAsyncAlwaysRunUpdateCommandHandler<RegisterPageTemplatesAndModuleTypesCommand>
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserContextService _userContextService;
        private readonly IExecutionContextFactory _executionContextFactory;

        public RegisterPageTemplatesAndModuleTypesCommandHandler(
            ICommandExecutor commandExecutor,
            IUserContextService userContextService,
            IExecutionContextFactory executionContextFactory
            )
        {
            _commandExecutor = commandExecutor;
            _userContextService = userContextService;
            _executionContextFactory = executionContextFactory;
        }


        public async Task ExecuteAsync(RegisterPageTemplatesAndModuleTypesCommand command)
        {
            var cx = await _executionContextFactory.CreateSystemUserContextAsync();
            await _commandExecutor.ExecuteAsync(new RegisterPageTemplatesCommand(), cx);
            await _commandExecutor.ExecuteAsync(new RegisterPageModuleTypesCommand(), cx);
        }
    }
}
