using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Marks the user as deleted in the database (soft delete) and signs them out. Fields
    /// containing personal data are cleared and any optional relations from the UnstructuredDataDependency
    /// table are deleted. The remaining user record and relations are left in place for auditing.
    /// Log tables that contain IP references are not deleted, but should be
    /// cleared out periodically by the <see cref="BackgroundTasks.UserCleanupBackgroundTask"/>.
    /// </summary>
    public class DeleteCurrentUserCommandHandler
        : ICommandHandler<DeleteCurrentUserCommand>
        , IPermissionRestrictedCommandHandler<DeleteCurrentUserCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IPermissionValidationService _permissionValidationService;

        public DeleteCurrentUserCommandHandler(
            IDomainRepository domainRepository,
            IPermissionValidationService permissionValidationService
            )
        {
            _domainRepository = domainRepository;
            _permissionValidationService = permissionValidationService;
        }

        public async Task ExecuteAsync(DeleteCurrentUserCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);
            var userId = executionContext.UserContext.UserId.Value;

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _domainRepository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(new DeleteUserCommand(userId));

                await _domainRepository
                    .ExecuteCommandAsync(new SignOutCurrentUserCommand());

                await scope.CompleteAsync();
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteCurrentUserCommand command)
        {
            yield return new CurrentUserDeletePermission();
        }
    }
}