using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class IPermissionExtensions
    {
        public static string GetUniqueCode(this IPermission permission)
        {
            if (permission.PermissionType == null) return null;

            if (permission is IEntityPermission)
            {
                var customEntityPermission = (IEntityPermission)permission;
                if (customEntityPermission.EntityDefinition == null) return null;

                return customEntityPermission.EntityDefinition.EntityDefinitionCode + permission.PermissionType.Code;
            }

            return permission.PermissionType.Code;
        }
    }
}
