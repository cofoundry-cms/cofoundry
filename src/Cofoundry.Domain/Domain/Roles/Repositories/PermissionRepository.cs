namespace Cofoundry.Domain.Internal;

public class PermissionRepository : IPermissionRepository
{
    private readonly IDictionary<string, IPermission> _permissions;

    public PermissionRepository(
        IEnumerable<IPermission> permissions,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        var customEntityPermissions = GetCustomEntityPermissions(permissions, customEntityDefinitionRepository).ToList();

        var allPermissions = permissions
            .Where(p => !(p is ICustomEntityPermissionTemplate))
            .Union(customEntityPermissions);

        DetectDuplicates(allPermissions);
        _permissions = allPermissions.ToDictionary(k => GetUniqueKey(k));
    }

    private void DetectDuplicates(IEnumerable<IPermission> permissions)
    {
        var dulpicateCodes = permissions
            .GroupBy(e => GetUniqueKey(e), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);

        if (dulpicateCodes.Any())
        {
            throw new Exception("Duplicate IPermission.CustomEntityDefinitionCode: " + dulpicateCodes.First().Key);
        }
    }

    private IEnumerable<IPermission> GetCustomEntityPermissions(IEnumerable<IPermission> permissions, ICustomEntityDefinitionRepository customEntityDefinitionRepository)
    {
        var customEntityDefinitions = customEntityDefinitionRepository.GetAll();

        foreach (var permissionToTransform in permissions
            .Where(p => p is ICustomEntityPermissionTemplate))
        {
            foreach (var customEntityDefinition in customEntityDefinitions)
            {
                var customEntityPermissions = ((ICustomEntityPermissionTemplate)permissionToTransform).CreateImplemention(customEntityDefinition);
                yield return customEntityPermissions;
            }
        }
    }

    public IPermission GetByCode(string permissionTypeCode, string entityDefinitionCode)
    {
        var key = PermissionIdentifierFormatter.GetUniqueIdentifier(permissionTypeCode, entityDefinitionCode);
        return _permissions.GetOrDefault(key);
    }

    public IEnumerable<IPermission> GetAll()
    {
        return _permissions.Select(p => p.Value);
    }

    public IPermission GetByEntityAndPermissionType(IEntityDefinition entityDefinition, PermissionType permissionType)
    {
        if (entityDefinition == null || permissionType == null) return null;
        return GetByEntityAndPermissionType(entityDefinition.EntityDefinitionCode, permissionType.Code);
    }

    public IPermission GetByEntityAndPermissionType(string entityDefinitionCode, string permissionTypeCode)
    {
        if (string.IsNullOrEmpty(entityDefinitionCode) || string.IsNullOrEmpty(permissionTypeCode)) return null;
        var key = PermissionIdentifierFormatter.GetUniqueIdentifier(permissionTypeCode, entityDefinitionCode);
        return _permissions.GetOrDefault(key);
    }

    private string GetUniqueKey(IPermission permission)
    {
        if (permission == null) return null;

        return PermissionIdentifierFormatter.GetUniqueIdentifier(permission);
    }
}
