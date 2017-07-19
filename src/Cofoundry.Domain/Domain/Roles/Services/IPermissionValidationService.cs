using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Centralised service containging helper methods for handling permission checks.
    /// </summary>
    public interface IPermissionValidationService
    {
        /// <summary>
        /// Checks to see if the currently logged in user is in the super administrator role,
        /// if not, throws an exception.
        /// </summary>
        Task EnforceIsSuperAdminRoleAsync();

        /// <summary>
        /// Checks to see if the specified user context is in the super administrator role,
        /// if not, throws an exception.
        /// </summary>
        void EnforceIsSuperAdminRole(IUserContext userContext);

        /// <summary>
        /// Checks to see if the user if logged in and throws a NotPermittedException if not.
        /// </summary>
        Task EnforceIsLoggedInAsync();

        /// <summary>
        /// Checks to see if the specified user context is logged in and throws a NotPermittedException if not.
        /// </summary>
        void EnforceIsLoggedIn(IUserContext userContext);

        /// <summary>
        /// Checks to see if the user has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        Task EnforceHasPermissionToUserAreaAsync(string userAreaCode);

        /// <summary>
        /// Checks to see if the specified user context has permission to the specified user area. Note that Cofoundry users
        /// have permissions to any user area
        /// </summary>
        void EnforceHasPermissionToUserArea(string userAreaCode, IUserContext userContext);

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified.
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        Task<bool> IsCurrentUserOrHasPermissionAsync<TPermission>(int userId) where TPermission : IPermissionApplication, new();

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified.
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        /// <param name="currentUserContext">An IUserContext representing the currently logged in user.</param>
        bool IsCurrentUserOrHasPermission<TPermission>(int userId, IUserContext currentUserContext) where TPermission : IPermissionApplication, new();

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified. If the condition is not
        /// met a NotPermitted exception is thrown
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        Task EnforceCurrentUserOrHasPermissionAsync<TPermission>(int userId) where TPermission : IPermissionApplication, new();

        /// <summary>
        /// Determintes if the specified user id belongs to the current user or if the
        /// currenty logged in user has the specified permission. Useful for checking
        /// access to a user object when only an id is specified. If the condition is not
        /// met a NotPermitted exception is thrown
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check for if the id is not the currently logged in user</typeparam>
        /// <param name="userId">UserId to compare with the currently logged in user</param>
        /// <param name="currentUserContext">An IUserContext representing the currently logged in user.</param>
        void EnforceCurrentUserOrHasPermission<TPermission>(int userId, IUserContext currentUserContext) where TPermission : IPermissionApplication, new();

        Task<bool> HasPermissionAsync(IPermissionApplication permission);
        bool HasPermission(IPermissionApplication permission, IUserContext userContext);

        Task<bool> HasPermissionAsync<TPermission>() where TPermission : IPermissionApplication, new();
        bool HasPermission<TPermission>(IUserContext userContext) where TPermission : IPermissionApplication, new();

        Task EnforcePermissionAsync(IEnumerable<IPermissionApplication> permissions);
        bool HasPermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext);

        Task EnforcePermissionAsync(IPermissionApplication permission);
        void EnforcePermission(IPermissionApplication permission, IUserContext userContext);

        Task EnforcePermissionAsync<TPermission>() where TPermission : IPermissionApplication, new();
        void EnforcePermission<TPermission>(IUserContext userContext) where TPermission : IPermissionApplication, new();

        Task<bool> HasPermissionAsync(IEnumerable<IPermissionApplication> permissions);
        void EnforcePermission(IEnumerable<IPermissionApplication> permissions, IUserContext userContext);

        Task<bool> HasCustomEntityPermissionAsync<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new();
        bool HasCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new();

        Task EnforceCustomEntityPermissionAsync<TPermission>(IEnumerable<string> definitionCodes) where TPermission : ICustomEntityPermissionTemplate, new();
        void EnforceCustomEntityPermission<TPermission>(IEnumerable<string> definitionCodes, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new();

        Task EnforceCustomEntityPermissionAsync<TPermission>(string definitionCode) where TPermission : ICustomEntityPermissionTemplate, new();
        void EnforceCustomEntityPermission<TPermission>(string definitionCode, IUserContext userContext) where TPermission : ICustomEntityPermissionTemplate, new();
    }
}
