using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service to abstract away the management of a users current session
    /// e.g. for a web session this may be managed via a session cookie.
    /// </summary>
    public interface IUserSessionService
    {
        /// <summary>
        /// Gets the UserId of the user authenticated for the current request under 
        /// the ambient authentication scheme. The ambient scheme is usually the
        /// default scheme, but can be changed in scenarios with multiple user areas.
        /// </summary>
        /// <returns>
        /// Integer UserId or <see langword="null"/> if the user is not logged in for the ambient
        /// authentication scheme.
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

        /// <summary>
        /// Change the ambient user area from the one defined as
        /// the default.
        /// </summary>
        /// <param name="userAreaCode">
        /// The code identifier for the user area to change the defaut to.
        /// </param>
        /// <returns></returns>
        Task SetAmbientUserAreaAsync(string userAreaCode);

        /// <summary>
        /// Regenerates the user session, refreshing any cached data
        /// such e.g. claims such as the security token. If the specified
        /// user is not logged in then no action is taken.
        /// </summary>
        /// <param name="userAreaCode">
        /// Unique code of the user area the user belongs to e.g. "COF" for the Cofundry admin user area.
        /// </param>
        /// <param name="userId">Database id of the user to refresh the session for.</param>
        Task RefreshLoginAsync(string userAreaCode, int userId);
    }
}
