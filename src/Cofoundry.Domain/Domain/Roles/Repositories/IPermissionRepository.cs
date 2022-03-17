namespace Cofoundry.Domain;

public interface IPermissionRepository
{
    IPermission GetByCode(string permissionTypeCode, string entityDefinitionCode);
    IPermission GetByEntityAndPermissionType(IEntityDefinition entityDefinition, PermissionType permissionType);
    IPermission GetByEntityAndPermissionType(string entityDefinitionCode, string permissionTypeCode);
    IEnumerable<IPermission> GetAll();
}
