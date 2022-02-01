using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for retreiving information about the currently logged in user.
    /// By default the <see cref="IUserContext"/> instances are cached for the 
    /// lifetime of the service (per request in web apps).
    /// </summary>
    public interface IUserContextService
    {
        /// <summary>
        /// <para>
        /// Gets basic information about the currently logged in user. If the user is not 
        /// logged in then <see cref="UserContext.Empty"/> is returned. If multiple user
        /// areas are implemented, then the returned user will depend on the "ambient" 
        /// auth scheme, which is typically the default user area unless the ambient scheme 
        /// has been changed during the flow of the request e.g. via an AuthorizeUserAreaAttribute
        /// </para>
        /// <para>
        /// By default the <see cref="IUserContext"/> is cached for the lifetime of the service 
        /// (per request in web apps).
        /// </para>
        /// </summary>
        Task<IUserContext> GetCurrentContextAsync();

        /// <summary>
        /// <para>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges.
        /// </para>
        /// <para>
        /// By default the <see cref="IUserContext"/> is cached for the lifetime of the service 
        /// (per request in web apps).
        /// </para>
        /// </summary>
        Task<IUserContext> GetSystemUserContextAsync();

        /// <summary>
        /// <para>
        /// Gets basic information about the currently logged in user for a specific user area. 
        /// If the user is not logged in to the user area then <see cref="UserContext.Empty"/> is 
        /// returned. This can be useful in multi-user-area apps where the current or "ambient" context
        /// may not be the one you need.
        /// </para>
        /// <para>
        /// By default the <see cref="IUserContext"/> is cached for the lifetime of the service 
        /// (per request in web apps).
        /// </para>
        /// </summary>
        Task<IUserContext> GetCurrentContextByUserAreaAsync(string userAreaCode);
    }
}
