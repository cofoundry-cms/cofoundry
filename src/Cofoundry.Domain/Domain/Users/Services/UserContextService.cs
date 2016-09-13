using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Core;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
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

        public IUserContext GetCurrentContext()
        {
            if (!_isUserContextCached)
            {
                var userId = _userSessionService.GetCurrentUserId();
                SetUserContext(userId);
            }

            return _userContext;
        }

        private void SetUserContext(int? userId)
        {
            UserContext cx = null;

            if (userId.HasValue)
            {
                // Raw query required here because using IQueryExecutor will cause a stack overflow
                var dbResult = _dbContext
                    .Users
                    .AsNoTracking()
                    .FilterById(userId.Value)
                    .SingleOrDefault();

                if (dbResult == null)
                {
                    // User no longer valid
                    _userSessionService.Abandon();
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

        /// <summary>
        /// Use this to get a user context for the super admin, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges
        /// </summary>
        public async Task<IUserContext> GetSuperAdminUserContextAsync()
        {
            // Grab the first super admin user.
            var dbUser = await QuerySuperAdminUser().FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbUser, SpecialistRoleTypeCodes.SuperAdministrator);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            return impersonatedUserContext;
        }

        /// <summary>
        /// Use this to get a user context for the super admin, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges
        /// </summary>
        public IUserContext GetSuperAdminUserContext()
        {
            // Grab the first super admin user.
            var dbUser = QuerySuperAdminUser().FirstOrDefault();
            EntityNotFoundException.ThrowIfNull(dbUser, SpecialistRoleTypeCodes.SuperAdministrator);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            return impersonatedUserContext;
        }

        public void ClearCache()
        {
            _isUserContextCached = false;
            _userContext = null;
        }

        #region helpers

        private IQueryable<User> QuerySuperAdminUser()
        {
            var query = _dbContext
                .Users
                .FilterByUserArea(CofoundryAdminUserArea.AreaCode)
                .FilterActive()
                .Where(u => u.Role.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.SuperAdministrator);

            return query;
        }

        #endregion
    }
}
