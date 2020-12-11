using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
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
        public virtual async Task EnforceIsSuperAdminRoleAsync()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforceIsSuperAdminRole(userContext);
        }

        /// <summary>
        /// Checks to see if the specified user context is in the super administrator role,
        /// if not, throws an exception.
        /// </summary>
        public virtual void EnforceIsSuperAdminRole(IUserContext userContext)
        {
            bool isSuperAdmin = false;

            if (userContext != null && userContext.UserArea is CofoundryAdminUserArea)
            {
                isSuperAdmin = userContext.RoleCode == SuperAdminRole.SuperAdminRoleCode;
            }

            if (!isSuperAdmin)
            {
                throw new NotPermittedException("SuperAdmin access is required to perform this action");
            }
        }

        /// <summary>
        /// Checks to see if the user if logged in and throws a NotPermittedException if not.
        /// </summary>
        public virtual async Task EnforceIsLoggedInAsync()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforceIsLoggedIn(userContext);
        }

        /// <summary>
        /// Checks to see if the specified user context is logged in and throws a NotPermittedException if not.
        /// </summary>
        public virtual void EnforceIsLoggedIn(IUserContext userContext)
        {
            ThrowExceptionIfNotLoggedIn(userContext);
        }

        /// <summary>
        /// Checks to see if the user has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        public virtual async Task EnforceHasPermissionToUserAreaAsync(string userAreaCode)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforceHasPermissionToUserArea(userAreaCode, userContext);
        }

        /// <summary>
        /// Checks to see if the specified user context has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        public virtual void EnforceHasPermissionToUserArea(string userAreaCode, IUserContext userContext)
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
        public virtual async Task<bool> IsCurrentUserOrHasPermissionAsync<TPermission>(int userId) where TPermission : IPermissionApplication, new()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            return IsCurrentUserOrHasPermission<TPermission>(userId, userContext);
        }

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified.
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        /// <param name="currentUserContext">An IUserContext representing the currently logged in user.</param>
        public virtual bool IsCurrentUserOrHasPermission<TPermission>(int userId, IUserContext currentUserContext) where TPermission : IPermissionApplication, new()
        {
            if (currentUserContext == null) return false;

            bool isPermitted = currentUserContext.UserId == userId || HasPermission<TPermission>(currentUserContext);

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
        public virtual async Task EnforceCurrentUserOrHasPermissionAsync<TPermission>(int userId) where TPermission : IPermissionApplication, new()
        {
            var hasPermission = await IsCurrentUserOrHasPermissionAsync<TPermission>(userId);
            if (!hasPermission)
            {
                var userContext = await _userContextService.GetCurrentContextAsync();
                throw new PermissionValidationFailedException(new TPermission(), userContext);
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
        /// <param name="currentUserContext">An IUserContext representing the currently logged in user.</param>
        public virtual void EnforceCurrentUserOrHasPermission<TPermission>(int userId, IUserContext currentUserContext) where TPermission : IPermissionApplication, new()
        {
            if (!IsCurrentUserOrHasPermission<TPermission>(userId, currentUserContext))
            {
                throw new PermissionValidationFailedException(new TPermission(), currentUserContext);
            }
        }

        public virtual Task<bool> HasPermissionAsync<TPermission>() where TPermission : IPermissionApplication, new()
        {
            return HasPermissionAsync(new TPermission());
        }

        public virtual bool HasPermission<TPermission>(IUserContext userContext) where TPermission : IPermissionApplication, new()
        {
            return HasPermission(new TPermission(), userContext);
        }

        public virtual async Task<bool> HasPermissionAsync(IPermissionApplication permissionApplication)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();

            return HasPermission(permissionApplication, userContext);
        }

        public virtual bool HasPermission(IPermissionApplication permissionApplication, IUserContext userContext)
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

        public virtual async Task<bool> HasPermissionAsync(IEnumerable<IPermissionApplication> permissions)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            return HasPermission(permissions, userContext);
        }

        public virtual bool HasPermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext)
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

        public virtual async Task EnforcePermissionAsync(IPermissionApplication permission)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforcePermission(permission, userContext);
        }

        public virtual void EnforcePermission(IPermissionApplication permission, IUserContext userContext)
        {
            if (!HasPermission(permission, userContext))
            {
                throw new PermissionValidationFailedException(permission, userContext);
            }
        }

        public virtual async Task EnforcePermissionAsync<TPermission>() where TPermission : IPermissionApplication, new()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforcePermission<TPermission>(userContext);
        }

        public virtual void EnforcePermission<TPermission>(IUserContext userContext) where TPermission : IPermissionApplication, new()
        {
            var permission = new TPermission();
            if (!HasPermission(permission, userContext))
            {
                throw new PermissionValidationFailedException(permission, userContext);
            }
        }

        public virtual async Task EnforcePermissionAsync(IEnumerable<IPermissionApplication> permissions)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforcePermission(permissions, userContext);
        }

        public virtual void EnforcePermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext)
        {
            foreach (var permission in permissions)
            {
                EnforcePermission(permission, userContext);
            }
        }

        public virtual async Task<bool> HasCustomEntityPermissionAsync<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            return HasCustomEntityPermission<TPermission>(definitionCode, userContext);
        }

        public virtual bool HasCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var permission = GetCustomEntityPermission<TPermission>(definitionCode);

            return HasPermission(permission, userContext);
        }

        public virtual async Task EnforceCustomEntityPermissionAsync<TPermission>(IEnumerable<string> definitionCodes) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforceCustomEntityPermission<TPermission>(definitionCodes, userContext);
        }

        public virtual void EnforceCustomEntityPermission<TPermission>(IEnumerable<string> definitionCodes, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            foreach (var definitionCode in definitionCodes.Distinct())
            {
                EnforceCustomEntityPermission<TPermission>(definitionCode, userContext);
            }
        }

        public virtual async Task EnforceCustomEntityPermissionAsync<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new()
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            EnforceCustomEntityPermission<TPermission>(definitionCode, userContext);
        }

        public virtual void EnforceCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new()
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
                throw new NotPermittedException("This operation requires that a user is logged in");
            }
        }

        #endregion
        
    }
}
