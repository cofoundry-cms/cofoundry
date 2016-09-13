using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PermissionEqualityComparer : IEqualityComparer<IPermission>
    {
        public bool Equals(IPermission x, IPermission y)
        {
            if (x == null || y == null) return false;

            // These should never be null
            if (x.PermissionType == null || y.PermissionType == null) return false;

            if (x is IEntityPermission != y is IEntityPermission) return false;

            // compare entity definition code
            var xDef = GetEntityDefinitionCode(x);
            if (xDef != null && xDef != GetEntityDefinitionCode(y)) return false;

            return x.PermissionType.Code == y.PermissionType.Code;
        }

        private string GetEntityDefinitionCode(IPermission permission)
        {
            var entityPermission = permission as IEntityPermission;
            if (entityPermission != null && entityPermission.EntityDefinition != null)
            {
                return entityPermission.EntityDefinition.EntityDefinitionCode;
            }

            return null;
        }

        public int GetHashCode(IPermission obj)
        {
            return obj.GetHashCode();
        }
    }
}
