using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Service for retreiving user connection information.
    /// </summary>
    public class UserContextService : IUserContextService
    {
        /// <summary>
        /// Unless logging in or out, the user context is bound to the lifetime
        /// of the request/service, therefore we cache it because it is regularly
        /// accessed from many different places.
        /// </summary>
        private IUserContext _currentUserContext = null;
        private Dictionary<string, IUserContext> _alternativeUserContextCache = new Dictionary<string, IUserContext>();

        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly UserContextMapper _userContextMapper;

        public UserContextService(
            CofoundryDbContext dbContext,
            IUserSessionService userSessionService,
            UserContextMapper userContextMapper
            )
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _userContextMapper = userContextMapper;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get the connection context of the current user.
        /// </summary>
        public virtual async Task<IUserContext> GetCurrentContextAsync()
        {
            if (_currentUserContext == null)
            {
                var userId = _userSessionService.GetCurrentUserId();
                await SetUserContextAsync(userId);
            }

            return _currentUserContext;
        }

        /// <summary>
        /// Get the connection context of the current user for a specific 
        /// user area. This can be useful in multi-userarea sites where the ambient
        /// context may not be the context you need.
        /// </summary>
        /// <remarks>
        /// In Cofoundry we use this to get the user context of a logged in Cofoundry
        /// user for the site viewer, irrespective of the ambient user context, which 
        /// potentially could be (for example) a members area.
        /// </remarks>
        public virtual async Task<IUserContext> GetCurrentContextByUserAreaAsync(string userAreaCode)
        {
            var userContext = _alternativeUserContextCache.GetOrDefault(userAreaCode);
            if (userContext != null) return userContext;

            userContext = await GetCurrentContextAsync();

            // if the current context is valid, return that
            if (userContext != null && userContext.UserId.HasValue && userContext.UserArea?.UserAreaCode == userAreaCode)
            {
                return userContext;
            }

            // otherwise, try and get a specific user context:
            var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
            if (!userId.HasValue) return new UserContext();

            userContext = await GetUserContextByIdAsync(userId);

            if (!userContext.UserId.HasValue)
            {
                // User no longer valid, clear out invalid login
                await _userSessionService.LogUserOutAsync(userAreaCode);
                ClearCache();
            }

            _alternativeUserContextCache.Add(userAreaCode, userContext);

            return userContext;
        }

        /// <summary>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges.
        /// </summary>
        public virtual async Task<IUserContext> GetSystemUserContextAsync()
        {
            // BUG: Got a managed debugging assistant exception? Try this:
            // https://developercommunity.visualstudio.com/content/problem/29782/managed-debugging-assistant-fatalexecutionengineer.html

            var dbUser = await QuerySystemUser().FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbUser, SuperAdminRole.SuperAdminRoleCode);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            return impersonatedUserContext;
        }

        /// <summary>
        /// Clears out the cached user context if one exists. The user 
        /// context is cached for the duration of the request so it needs clearing if
        /// it changes (i.e. logged in or out).
        /// </summary>
        public virtual void ClearCache()
        {
            _currentUserContext = null;
        }

        #endregion

        #region helpers

        protected virtual async Task SetUserContextAsync(int? userId)
        {
            var cx = await GetUserContextByIdAsync(userId);

            if (userId.HasValue && !cx.UserId.HasValue)
            {
                // User no longer valid, clear out all logins to be safe
                await _userSessionService.LogUserOutOfAllUserAreasAsync();
                ClearCache();
            }

            _currentUserContext = cx;
        }

        protected virtual async Task<UserContext> GetUserContextByIdAsync(int? userId)
        {
            if (!userId.HasValue) return new UserContext();

            UserContext cx = null;

            // Raw query required here because using IQueryExecutor will cause a stack overflow
            var dbResult = await _dbContext
                .Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FilterById(userId.Value)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();

            if (dbResult != null)
            {
                cx = _userContextMapper.Map(dbResult);
            }
            else
            {
                cx = new UserContext();
            }

            return cx;
        }

        protected virtual IQueryable<User> QuerySystemUser()
        {
            var query = _dbContext
                .Users
                .Include(u => u.Role)
                .FilterByUserArea(CofoundryAdminUserArea.AreaCode)
                .FilterActive()
                .Where(u => u.IsSystemAccount);

            return query;
        }

        #endregion
    }
}
