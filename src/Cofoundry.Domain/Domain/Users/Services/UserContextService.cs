using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserContextService : IUserContextService
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly UserContextMapper _userContextMapper;
        private readonly IUserContextCache _userContextCache;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UserContextService(
            CofoundryDbContext dbContext,
            IUserSessionService userSessionService,
            UserContextMapper userContextMapper,
            IUserContextCache userContextCache,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _userContextMapper = userContextMapper;
            _userContextCache = userContextCache;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }


        public virtual async Task<IUserContext> GetCurrentContextAsync()
        {
            var userId = _userSessionService.GetCurrentUserId();
            var userContext = await GetUserContextByIdAsync(userId);

            return userContext;
        }

        public virtual async Task<IUserContext> GetCurrentContextByUserAreaAsync(string userAreaCode)
        {
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));

            var userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
            var userContext = await GetUserContextByIdAsync(userId);

            return userContext;
        }

        public async Task<IUserContext> GetSystemUserContextAsync()
        {
            var userContext = await _userContextCache.GetOrAddSystemContextAsync(QuerySystemUserContextAsync);

            return userContext;
        }

        /// <summary>
        /// Queries the database for the system user and returns the result as an <see cref="IUserContext"/>
        /// projection. The result of this is cached in the <see cref="GetSystemUserContextAsync"/> method.
        /// </summary>
        protected virtual async Task<IUserContext> QuerySystemUserContextAsync()
        {
            var dbUser = await _dbContext
                .Users
                .Include(u => u.Role)
                .FilterByUserArea(CofoundryAdminUserArea.AreaCode)
                .FilterActive()
                .Where(u => u.IsSystemAccount)
                .FirstOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbUser, SuperAdminRole.SuperAdminRoleCode);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            return impersonatedUserContext;
        }

        private async Task<IUserContext> GetUserContextByIdAsync(int? userId)
        {
            if (!userId.HasValue) return UserContext.Empty;

            var userContext = await _userContextCache.GetOrAddAsync(userId.Value, () => QueryUserContextByIdAsync(userId.Value));

            return userContext;
        }

        /// <summary>
        /// Queries the database for the specified user and returns the result as an <see cref="IUserContext"/>
        /// projection. The result of this is cached in the <see cref="GetUserContextByIdAsync"/> method.
        /// </summary>
        protected virtual async Task<IUserContext> QueryUserContextByIdAsync(int userId)
        {
            IUserContext cx = null;

            // Raw query required here because using IQueryExecutor will cause a stack overflow
            var dbResult = await _dbContext
                .Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();

            if (dbResult != null)
            {
                cx = _userContextMapper.Map(dbResult);
            }
            else
            {
                cx = UserContext.Empty;

                // User no longer valid, clear out all logins to be safe
                await _userSessionService.LogUserOutOfAllUserAreasAsync();
            }

            return cx;
        }
    }
}
