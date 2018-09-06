using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service to abstract away the management of a users current browser session and 
    /// authentication cookie.
    /// </summary>
    public interface IUserSessionService
    {
        /// <summary>
        /// Gets the UserId of the user currently logged
        /// in to this session
        /// </summary>
        /// <returns>
        /// UserId of the user currently logged
        /// in to this session
        /// </returns>
        int? GetCurrentUserId();

        /// <summary>
        /// Gets the UserId of the currently logged in user for a specific UserArea,
        /// regardless of the ambient authentication scheme. Useful in multi-userarea
        /// scenarios where you need to ignore the ambient user and check for permissions 
        /// against a specific user area.
        /// </summary>
        /// <param name="userAreaCode">The unique identifying code of the user area to check for.</param>
        Task<int?> GetUserIdByUserAreaCodeAsync(string userAreaCode);

        /// <summary>
        /// Logs the specified UserId into the current session.
        /// </summary>
        /// <param name="userAreaDefinitionCode">
        /// Unique code of the user area to log the user into (required).
        /// </param>
        /// <param name="userId">UserId belonging to the owner of the current session.</param>
        /// <param name="rememberUser">
        /// True if the session should last indefinately; false if the 
        /// session should close after a timeout period.
        /// </param>
        Task LogUserInAsync(string userAreaDefinitionCode, int userId, bool rememberUser);

        /// <summary>
        /// Logs the user out of the specified user area.
        /// </summary>
        /// <param name="userAreaCode">Unique code of the user area to log the user out of (required).</param>
        Task LogUserOutAsync(string userAreaDefinitionCode);

        /// <summary>
        /// Logs the user out of all user areas.
        /// </summary>
        Task LogUserOutOfAllUserAreasAsync();
    }
}
