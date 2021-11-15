using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for <see cref="IUserContext"/> projections, which are frequently accessed
    /// throughout the framework during a DI scope or web request. Unlike most caches in
    /// Cofoundry, this cache is linked to the duration of the current DI scope (e.g. web 
    /// request).
    /// </summary>
    public interface IUserContextCache
    {
        /// <summary>
        /// Gets a user context if it's already cached, otherwise the <paramref name="getter"/> is invoked
        /// and the result is cached and returned.
        /// </summary>
        /// <param name="userId">Database Id of the user to return.</param>
        /// <param name="getter">Function to invoke if the user context isn't in the cache.</param>
        Task<IUserContext> GetOrAddAsync(int userId, Func<Task<IUserContext>> getter);

        /// <summary>
        /// Gets a <see cref="IUserContext"/> projection of the system user if it's already 
        /// cached, otherwise the <paramref name="getter"/> is invoked and the result is cached 
        /// and returned.
        /// </summary>
        /// <param name="getter">Function to invoke if the system user context isn't in the cache.</param>
        Task<IUserContext> GetOrAddSystemContextAsync(Func<Task<IUserContext>> getter);


        /// <summary>
        /// Clears the cache entry for the specified user only.
        /// </summary>
        /// <param name="userId">Id of the user to clear cache entries for.</param>
        void Clear(int userId);

        /// <summary>
        /// Clears all items in the cache.
        /// </summary>
        void Clear();
    }
}
