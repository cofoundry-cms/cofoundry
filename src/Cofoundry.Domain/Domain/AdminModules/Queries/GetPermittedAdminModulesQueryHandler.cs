namespace Cofoundry.Domain.Internal;

public class GetPermittedAdminModulesQueryHandler
    : IQueryHandler<GetPermittedAdminModulesQuery, IReadOnlyCollection<AdminModule>>
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

    public Task<IReadOnlyCollection<AdminModule>> ExecuteAsync(GetPermittedAdminModulesQuery query, IExecutionContext executionContext)
    {
        var userContext = executionContext.UserContext;

        if (userContext == null || !userContext.IsCofoundryUser())
        {
            return Task.FromResult<IReadOnlyCollection<AdminModule>>(Array.Empty<AdminModule>());
        }

        var modules = _moduleRegistrations
            .SelectMany(r => r.GetModules())
            .Where(r => _permissionValidationService.HasPermission(r.RestrictedToPermission, executionContext.UserContext))
            .SetStandardOrdering()
            .ToList();

        return Task.FromResult<IReadOnlyCollection<AdminModule>>(modules);
    }
}
