using Microsoft.AspNetCore.Authorization;
using Cofoundry.Domain;
using System;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// Ensures that a class or method can only be accessed by users with a role that
    /// includes the specified permission.
    /// </summary>
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizePermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissionType">
        /// The type of the code-defined permission to restrict access to. The type must inherit
        /// from <see cref="IPermission"/> and have a public parameterless constructor.
        /// </param>
        public AuthorizePermissionAttribute(Type permissionType)
            : base()
        {
            if (permissionType == null) throw new ArgumentNullException(nameof(permissionType));

            if (permissionType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"{permissionType} must have a parameterless constructor to be used in {nameof(AuthorizePermissionAttribute)}");
            }

            if (typeof(ICustomEntityPermissionTemplate).IsAssignableFrom(permissionType))
            {
                throw new ArgumentException($"Permissions that implement {nameof(ICustomEntityPermissionTemplate)} cannot be used in {nameof(AuthorizePermissionAttribute)} without specifying an entity type. Use the construcotr overload that takes a custom entity definition code instead.");
            }

            if (!typeof(IPermission).IsAssignableFrom(permissionType))
            {
                throw new ArgumentException($"{permissionType} must be a permission type implementing {nameof(IPermission)} to be used in {nameof(AuthorizePermissionAttribute)}");
            }

            var permission = (IPermission)Activator.CreateInstance(permissionType);
            Policy = AuthorizationPolicyNames.Permission(permission);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizePermissionAttribute"/> class
        /// with a custom entity permission type.
        /// </summary>
        /// <param name="customEntityPermissionTemplateType">
        /// The type of the code-defined permission to restrict access to. The type must inherit
        /// from <see cref="ICustomEntityPermissionTemplate"/> and have a public parameterless constructor.
        /// </param>
        /// <param name="customEntityDefinitionCode">
        /// The 6 character custom entity definition code that the permission template should apply to.
        /// </param>
        public AuthorizePermissionAttribute(Type customEntityPermissionTemplateType, string customEntityDefinitionCode)
            : base()
        {
            if (customEntityPermissionTemplateType == null) throw new ArgumentNullException(nameof(customEntityPermissionTemplateType));
            if (string.IsNullOrWhiteSpace(customEntityDefinitionCode)) throw new ArgumentEmptyException(nameof(customEntityDefinitionCode));

            if (customEntityPermissionTemplateType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"{customEntityPermissionTemplateType} must have a parameterless constructor to be used in {nameof(AuthorizePermissionAttribute)}");
            }

            if (!typeof(ICustomEntityPermissionTemplate).IsAssignableFrom(customEntityPermissionTemplateType))
            {
                throw new ArgumentException($"{customEntityPermissionTemplateType} must be a permission type implementing {nameof(ICustomEntityPermissionTemplate)} to be used in {nameof(AuthorizePermissionAttribute)} with a {nameof(customEntityDefinitionCode)}");
            }

            var permission = (ICustomEntityPermissionTemplate)Activator.CreateInstance(customEntityPermissionTemplateType);
            if (permission.PermissionType == null) throw new InvalidOperationException(nameof(customEntityPermissionTemplateType));

            Policy = AuthorizationPolicyNames.Permission(permission.PermissionType.Code, customEntityDefinitionCode);
        }
    }
}