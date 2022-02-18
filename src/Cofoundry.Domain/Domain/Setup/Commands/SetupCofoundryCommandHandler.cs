using Cofoundry.Core.Caching;
using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class SetupCofoundryCommandHandler
        : ICommandHandler<SetupCofoundryCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IObjectCacheFactory _objectCacheFactory;

        public SetupCofoundryCommandHandler(
            IDomainRepository domainRepository,
            IObjectCacheFactory objectCacheFactory
            )
        {
            _domainRepository = domainRepository.WithElevatedPermissions();
            _objectCacheFactory = objectCacheFactory;
        }

        public async Task ExecuteAsync(SetupCofoundryCommand command, IExecutionContext executionContext)
        {
            var settings = await _domainRepository.ExecuteQueryAsync(new GetSettingsQuery<InternalSettings>());

            if (settings.IsSetup)
            {
                throw new InvalidOperationException("Site is already set up.");
            }

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                var userId = await CreateAdminUser(command);

                var settingsCommand = await _domainRepository.ExecuteQueryAsync(new GetUpdateCommandQuery<UpdateGeneralSiteSettingsCommand>());
                settingsCommand.ApplicationName = command.ApplicationName;
                await _domainRepository.ExecuteCommandAsync(settingsCommand);

                // Take the opportunity to break the cache in case any additional install scripts have been run since initialization
                _objectCacheFactory.Clear();

                // Setup Complete
                await _domainRepository.ExecuteCommandAsync(new MarkAsSetUpCommand());

                await scope.CompleteAsync();
            }
        }

        private async Task<int> CreateAdminUser(SetupCofoundryCommand command)
        {
            var newUserCommand = new AddUserCommand()
            {
                Email = command.Email,
                DisplayName = command.DisplayName,
                Password = command.Password,
                RequirePasswordChange = command.RequirePasswordChange,
                UserAreaCode = CofoundryAdminUserArea.Code,
                RoleCode = SuperAdminRole.Code
            };

            await _domainRepository.ExecuteCommandAsync(newUserCommand);

            return newUserCommand.OutputUserId;
        }
    }
}
