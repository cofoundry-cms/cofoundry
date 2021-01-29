using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetPermittedAdminModulesQueryHandler
        : IQueryHandler<GetPermittedAdminModulesQuery, ICollection<AdminModule>>
        , IIgnorePermissionCheckHandler
    {
        private readonly IEnumerable<IAdminModuleRegistration> _moduleRegistrations;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetPermittedAdminModulesQueryHandler(
            IEnumerable<IAdminModuleRegistration> moduleRegistrations,
            IPermissionValidationService permissionValidationService
            )
        {
            _moduleRegistrations = moduleRegistrations;
            _permissionValidationService = permissionValidationService;
        }

        public Task<ICollection<AdminModule>> ExecuteAsync(GetPermittedAdminModulesQuery query, IExecutionContext executionContext)
        {
            var userContext = executionContext.UserContext;

            if (userContext == null || !userContext.IsCofoundryUser())
            {
                return Task.FromResult<ICollection<AdminModule>>(new AdminModule[0]);
            }

            var modules = _moduleRegistrations
                .SelectMany(r => r.GetModules())
                .Where(r => _permissionValidationService.HasPermission(r.RestrictedToPermission, executionContext.UserContext))
                .SetStandardOrdering()
                .ToList();

            return Task.FromResult<ICollection<AdminModule>>(modules);
        }
    }
}
