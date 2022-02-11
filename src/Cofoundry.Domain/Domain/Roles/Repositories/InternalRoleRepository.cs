using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class InternalRoleRepository : IInternalRoleRepository
    {
        private readonly IRoleCache _roleCache;
        private readonly CofoundryDbContext _dbContext;
        private readonly IRoleDetailsMapper _roleDetailsMapper;

        public InternalRoleRepository(
            IRoleCache roleCache,
            CofoundryDbContext dbContext,
            IRoleDetailsMapper roleDetailsMapper
            )
        {
            _roleCache = roleCache;
            _dbContext = dbContext;
            _roleDetailsMapper = roleDetailsMapper;
        }

        public RoleDetails GetById(int? roleId)
        {
            if (!roleId.HasValue) return GetAnonymousRoleFromCache();
            if (roleId < 1) return null;

            var cachedRole = _roleCache.GetOrAdd(roleId.Value, () =>
            {
                var dbRole = QueryRoles()
                    .FilterById(roleId.Value)
                    .SingleOrDefault();

                var role = _roleDetailsMapper.Map(dbRole);

                return role;
            });

            return cachedRole;
        }

        public async Task<RoleDetails> GetByIdAsync(int? roleId)
        {
            if (!roleId.HasValue) return await GetAnonymousRoleFromCacheAsync();
            if (roleId < 1) return null;

            var cachedRole = await _roleCache.GetOrAddAsync(roleId.Value, async () =>
            {
                var dbRole = await QueryRoles()
                    .FilterById(roleId.Value)
                    .SingleOrDefaultAsync();

                var result = _roleDetailsMapper.Map(dbRole);

                return result;
            });

            return cachedRole;
        }

        public async Task<IDictionary<int, RoleDetails>> GetByIdRangeAsync(IEnumerable<int> roleIds)
        {
            var cachedRoles = await _roleCache.GetOrAddRangeAsync(
                roleIds,
                GetMissingRolesAsync
            );

            return cachedRoles;

            async Task<ICollection<RoleDetails>> GetMissingRolesAsync(IEnumerable<int> missingRoleIds)
            {
                var dbRole = await QueryRoles()
                    .Where(r => roleIds.Contains(r.RoleId))
                    .ToListAsync();

                var result = dbRole
                    .Select(_roleDetailsMapper.Map)
                    .ToList();

                return result;
            }
        }

        private RoleDetails GetAnonymousRoleFromCache()
        {
            return _roleCache.GetOrAddAnonymousRole(() =>
            {
                var dbRole = QueryAnonymousRole().SingleOrDefault();
                EntityNotFoundException.ThrowIfNull(dbRole, AnonymousRole.Code);
                var role = _roleDetailsMapper.Map(dbRole);

                return role;
            });
        }

        private Task<RoleDetails> GetAnonymousRoleFromCacheAsync()
        {
            return _roleCache.GetOrAddAnonymousRoleAsync(async () =>
            {
                var dbRole = await QueryAnonymousRole().SingleOrDefaultAsync();
                EntityNotFoundException.ThrowIfNull(dbRole, AnonymousRole.Code);
                var role = _roleDetailsMapper.Map(dbRole);

                return role;
            });
        }

        private IQueryable<Role> QueryAnonymousRole()
        {
            return QueryRoles()
                .Where(r => r.RoleCode == AnonymousRole.Code && r.UserAreaCode == CofoundryAdminUserArea.Code);

        }

        private IQueryable<Role> QueryRoles()
        {
            return _dbContext
                .Roles
                .AsNoTracking()
                .Include(r => r.UserArea)
                .Include(r => r.RolePermissions)
                .ThenInclude(p => p.Permission);
        }
    }
}
