using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Centralised service containging helper methods for handling permission checks. The 
    /// implementation mostly just piggybacks other services where the real logic happens.
    /// </summary>
    public class PermissionValidationService : IPermissionValidationService
    {
        #region constructor

        private readonly IUserContextService _userContextService;
        private readonly IInternalRoleRepository _internalRoleRepository;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public PermissionValidationService(
            IUserContextService userContextService,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IInternalRoleRepository internalRoleRepository
            )
        {
            _userContextService = userContextService;
            _internalRoleRepository = internalRoleRepository;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        /// <summary>
        /// Checks to see if the currently logged in user is in the super administrator role,
        /// if not, throws an exception.
        /// </summary>
        public void EnforceIsSuperAdminRole()
        {
            EnforceIsSuperAdminRole(_userContextService.GetCurrentContext());
        }

        /// <summary>
        /// Checks to see if the specified user context is in the super administrator role,
        /// if not, throws an exception.
        /// </summary>
        public void EnforceIsSuperAdminRole(IUserContext userContext)
        {
            bool isSuperAdmin = false;

            if (userContext != null && userContext.UserArea is CofoundryAdminUserArea)
            {
                var role = _internalRoleRepository.GetById(userContext.RoleId);
                isSuperAdmin = role != null && role.IsSuperAdministrator;
            }

            if (!isSuperAdmin)
            {
                throw new NotPermittedException("SuperAdmin access is required to perform this action");
            }
        }

        /// <summary>
        /// Checks to see if the user if logged in and throws a NotPermittedException if not.
        /// </summary>
        public void EnforceIsLoggedIn()
        {
            var userContext = _userContextService.GetCurrentContext();
            EnforceIsLoggedIn(userContext);
        }

        /// <summary>
        /// Checks to see if the specified user context is logged in and throws a NotPermittedException if not.
        /// </summary>
        public void EnforceIsLoggedIn(IUserContext userContext)
        {
            ThrowExceptionIfNotLoggedIn(userContext);
        }

        /// <summary>
        /// Checks to see if the user has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        public void EnforceHasPermissionToUserArea(string userAreaCode)
        {
            EnforceHasPermissionToUserArea(userAreaCode, _userContextService.GetCurrentContext());
        }

        /// <summary>
        /// Checks to see if the specified user context has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        public void EnforceHasPermissionToUserArea(string userAreaCode, IUserContext userContext)
        {
            ThrowExceptionIfNotLoggedIn(userContext);

            if (!userContext.IsCofoundryUser() 
                && (userContext.UserArea == null || userAreaCode != userContext.UserArea.UserAreaCode))
            {
                throw new NotPermittedException("Permission to access UserArea '" + userAreaCode + "' denied");
            }
        }

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified.
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        public bool IsCurrentUserOrHasPermission<TPermission>(int userId) where TPermission : IPermissionApplication, new()
        {
            return IsCurrentUserOrHasPermission< TPermission>(userId, _userContextService.GetCurrentContext());
        }

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified.
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        public bool IsCurrentUserOrHasPermission<TPermission>(int userId, IUserContext userContext) where TPermission : IPermissionApplication, new()
        {
            if (userContext == null) return false;

            bool isPermitted = userContext.UserId == userId || HasPermission<TPermission>();

            return isPermitted;
        }

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified. If the condition is not
        /// met a NotPermitted exception is thrown
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        public void EnforceCurrentUserOrHasPermission<TPermission>(int userId) where TPermission : IPermissionApplication, new()
        {
            if (!IsCurrentUserOrHasPermission<TPermission>(userId))
            {
                throw new PermissionValidationFailedException(new TPermission(), _userContextService.GetCurrentContext());
            }
        }

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified. If the condition is not
        /// met a NotPermitted exception is thrown
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        public void EnforceCurrentUserOrHasPermission<TPermission>(int userId, IUserContext currentUserContext) where TPermission : IPermissionApplication, new()
        {
            if (!IsCurrentUserOrHasPermission<TPermission>(userId))
            {
                throw new PermissionValidationFailedException(new TPermission(), currentUserContext);
            }
        }

        public bool HasPermission<TPermission>() where TPermission : IPermissionApplication, new()
        {
            return HasPermission(new TPermission());
        }

        public bool HasPermission<TPermission>(IUserContext userContext) where TPermission : IPermissionApplication, new()
        {
            return HasPermission(new TPermission(), userContext);
        }

        public bool HasPermission(IPermissionApplication permissionApplication)
        {
            return HasPermission(permissionApplication, _userContextService.GetCurrentContext());
        }

        public bool HasPermission(IPermissionApplication permissionApplication, IUserContext userContext)
        {
            if (permissionApplication == null) return true;

            if (userContext == null) return false;

            var role = _internalRoleRepository.GetById(userContext.RoleId);

            if (permissionApplication is IPermission)
            {
                return role.HasPermission((IPermission)permissionApplication);
            }
            else if (permissionApplication is CompositePermissionApplication)
            {
                foreach (var permission in ((CompositePermissionApplication)permissionApplication).Permissions)
                {
                    if (role.HasPermission(permission)) return true;
                }

                return false;
            }
            else
            {
                throw new InvalidOperationException("Unknown implementation of IPermissionApplication");
            }
        }

        public bool HasPermission(IEnumerable<IPermissionApplication> permissions)
        {
            return HasPermission(permissions, _userContextService.GetCurrentContext());
        }

        public bool HasPermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext)
        {
            foreach (var permission in permissions)
            {
                if (!HasPermission(permission, userContext))
                {
                    return false;
                }
            }

            return true;
        }

        public void EnforcePermission(IPermissionApplication permission)
        {
            EnforcePermission(permission, _userContextService.GetCurrentContext());
        }

        public void EnforcePermission(IPermissionApplication permission, IUserContext userContext)
        {
            if (!HasPermission(permission, userContext))
            {
                throw new PermissionValidationFailedException(permission, userContext);
            }
        }

        public void EnforcePermission(IEnumerable<IPermissionApplication> permissions)
        {
            EnforcePermission(permissions, _userContextService.GetCurrentContext());
        }

        public void EnforcePermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext)
        {
            foreach (var permission in permissions)
            {
                EnforcePermission(permission, userContext);
            }
        }

        public bool HasCustomEntityPermission<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            return HasCustomEntityPermission<TPermission>(definitionCode, _userContextService.GetCurrentContext());
        }

        public bool HasCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var permission = GetCustomEntityPermission<TPermission>(definitionCode);

            return HasPermission(permission, userContext);
        }

        public void EnforceCustomEntityPermission<TPermission>(IEnumerable<string> definitionCodes) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            EnforceCustomEntityPermission<TPermission>(definitionCodes, _userContextService.GetCurrentContext());
        }

        public void EnforceCustomEntityPermission<TPermission>(IEnumerable<string> definitionCodes, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            foreach (var definitionCode in definitionCodes.Distinct())
            {
                EnforceCustomEntityPermission<TPermission>(definitionCode, userContext);
            }
        }

        public void EnforceCustomEntityPermission<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            EnforceCustomEntityPermission<TPermission>(definitionCode, _userContextService.GetCurrentContext());
        }

        public void EnforceCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var permission = GetCustomEntityPermission<TPermission>(definitionCode);

            EnforcePermission(permission, userContext);
        }

        #region private helpers

        private ICustomEntityPermissionTemplate GetCustomEntityPermission<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var definition = _customEntityDefinitionRepository.GetByCode(definitionCode);
            var template = new TPermission();

            var permission = template.CreateImplemention(definition);
            return permission;
        }

        private static void ThrowExceptionIfNotLoggedIn(IUserContext userContext)
        {
            if (userContext == null || !userContext.UserId.HasValue)
            {
                throw new NotPermittedException("This operation required that a user is logged in");
            }
        }

        #endregion
        
    }
}
