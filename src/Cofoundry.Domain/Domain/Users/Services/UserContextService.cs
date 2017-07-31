using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for retreiving user connection information.
    /// </summary>
    public class UserContextService : IUserContextService
    {
        private bool _isUserContextCached = false;
        private UserContext _userContext = null;

        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly UserContextMapper _userContextMapper;

        public UserContextService(
            CofoundryDbContext dbContext,
            IUserSessionService userSessionService,
            UserContextMapper userContextMapper,
            IClientConnectionService browsingSessionService
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
        public async Task<IUserContext> GetCurrentContextAsync()
        {
            if (!_isUserContextCached)
            {
                var userId = _userSessionService.GetCurrentUserId();
                await SetUserContextAsync(userId);
            }

            return _userContext;
        }

        /// <summary>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges
        /// </summary>
        public async Task<IUserContext> GetSystemUserContextAsync()
        {
            // BUG: Got a managed debugging assistant exception? Try this:
            // https://developercommunity.visualstudio.com/content/problem/29782/managed-debugging-assistant-fatalexecutionengineer.html

            // Grab the first super admin user.
            var dbUser = await QuerySystemUser().FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbUser, SuperAdminRole.SuperAdminRoleCode);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            return impersonatedUserContext;
        }

        /// <summary>
        /// Clears out the cached user context if one exists. The user 
        /// context is cached for the duration of the request so it needs clearing if
        /// it changes (i.e. logged in.out)
        /// </summary>
        public void ClearCache()
        {
            _isUserContextCached = false;
            _userContext = null;
        }

        #endregion

        #region helpers

        private async Task SetUserContextAsync(int? userId)
        {
            UserContext cx = null;

            if (userId.HasValue)
            {
                // Raw query required here because using IQueryExecutor will cause a stack overflow
                var dbResult = await _dbContext
                    .Users
                    .AsNoTracking()
                    .FilterById(userId.Value)
                    .FilterCanLogIn()
                    .SingleOrDefaultAsync();

                if (dbResult == null)
                {
                    // User no longer valid
                    await _userSessionService.AbandonAsync();
                    ClearCache();
                }
                else
                {
                    cx = _userContextMapper.Map(dbResult);
                }
            }

            if (cx == null)
            {
                cx = new UserContext();
            }

            _userContext = cx;
            _isUserContextCached = true;
        }

        private IQueryable<User> QuerySystemUser()
        {
            var query = _dbContext
                .Users
                .FilterByUserArea(CofoundryAdminUserArea.AreaCode)
                .FilterActive()
                .Where(u => u.IsSystemAccount);

            return query;
        }

        #endregion
    }
}
