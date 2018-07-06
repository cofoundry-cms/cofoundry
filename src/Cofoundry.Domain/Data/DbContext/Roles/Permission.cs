using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// A permission represents an type action a user can
    /// be permitted to perform. Typically this is associated
    /// with a specified entity type, but doesn't have to be e.g.
    /// "read pages", "access dashboard", "delete images". The 
    /// combination of EntityDefinitionCode and PermissionCode
    /// must be unique
    /// </summary>
    public partial class Permission
    {
        public Permission()
        {
            RolePermissions = new List<RolePermission>();
        }

        /// <summary>
        /// Database id of the permission
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Optional code of the entity this permission relates to
        /// </summary>
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// A three letter code representing the action that is being permitted. The 
        /// combination of EntityDefinitionCode and PermissionCode must be unique 
        /// so some common permission codes can be used to reprsent common actions 
        /// like "read", "create" or "delete" when used in combination with a specific 
        /// entity type.
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// A permission can be assigned to many roles.
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Optional entity definition this permission relates to.
        /// </summary>
        public virtual EntityDefinition EntityDefinition { get; set; }

        /// <summary>
        /// Creates a unique code that represents this permission by combining the
        /// entity definition code and the permission code. This logic is replicated for
        /// IPermissions in IPermissionExtensions.GetUniqueCode
        /// </summary>
        /// <returns>A string code that uniquely identifies this permission</returns>
        public string GetUniqueCode()
        {
            var code = EntityDefinitionCode + PermissionCode;
            if (code == null) return null;

            return code.ToUpperInvariant();
        }
    }
}
