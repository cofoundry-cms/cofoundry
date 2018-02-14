using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPermissionRepository
    {
        IPermission GetByCode(string permissionTypeCode, string entityDefinitionCode);
        IPermission GetByEntityAndPermissionType(IEntityDefinition entityDefinition, PermissionType permissionType);
        IPermission GetByEntityAndPermissionType(string entityDefinitionCode, string permissionTypeCode);
        IEnumerable<IPermission> GetAll();
    }
}
