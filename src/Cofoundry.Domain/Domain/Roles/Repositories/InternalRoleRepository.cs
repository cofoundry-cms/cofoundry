using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Internal repository for fetching roles which bypasses CQS and permissions infrastructure
    /// to avoid circular references. Not to be used outside of Cofoundry core projects.
    /// </summary>
    /// <remarks>
    /// Not actually marked internal due to internal visibility restrictions and dependency injection
    /// </remarks>
    public class InternalRoleRepository : IInternalRoleRepository
    {
        #region constructor

        private readonly IRoleCache _roleCache;
        private readonly CofoundryDbContext _dbContext;
        private readonly IRoleDetailsMapper _roleMappingHelper;

        public InternalRoleRepository(
            IRoleCache roleCache,
            CofoundryDbContext dbContext,
            IRoleDetailsMapper roleMappingHelper
            )
        {
            _roleCache = roleCache;
            _dbContext = dbContext;
            _roleMappingHelper = roleMappingHelper;
        }

        #endregion

        #region public methods

        public RoleDetails GetById(int? roleId)
        {
            if (roleId == null || roleId < 1) return GetAnonymousRole();

            var cachedRole = _roleCache.GetOrAdd(roleId.Value, () =>
            {
                var dbRole = QueryRoleById(roleId.Value).FirstOrDefault();
                var role = _roleMappingHelper.Map(dbRole);

                return role;
            });

            if (cachedRole == null) return GetAnonymousRole();
            return cachedRole;
        }

        public async Task<RoleDetails> GetByIdAsync(int? roleId)
        {
            if (roleId == null || roleId < 1) return await GetAnonymousRoleAsync();

            var cachedRole = await _roleCache.GetOrAddAsync(roleId.Value, async () =>
            {
                var dbRole = await QueryRoleById(roleId.Value).FirstOrDefaultAsync();
                var result = _roleMappingHelper.Map(dbRole);

                return result;
            });

            if (cachedRole == null) return await GetAnonymousRoleAsync();
            return cachedRole;
        }

        #endregion

        #region private helpers

        private RoleDetails GetAnonymousRole()
        {
            return _roleCache.GetOrAddAnonymousRole(() =>
            {
                var dbRole = QueryAnonymousRole().FirstOrDefault();
                EntityNotFoundException.ThrowIfNull(dbRole, AnonymousRole.AnonymousRoleCode);
                var role = _roleMappingHelper.Map(dbRole);

                return role;
            });
        }

        private Task<RoleDetails> GetAnonymousRoleAsync()
        {
            return _roleCache.GetOrAddAnonymousRoleAsync(async () =>
            {
                var dbRole = await QueryAnonymousRole().FirstOrDefaultAsync();
                EntityNotFoundException.ThrowIfNull(dbRole, AnonymousRole.AnonymousRoleCode);
                var role = _roleMappingHelper.Map(dbRole);

                return role;
            });
        }

        private IQueryable<Role> QueryAnonymousRole()
        {
            return _dbContext
                    .Roles
                    .AsNoTracking()
                    .Include(r => r.UserArea)
                    .Include(r => r.RolePermissions)
                    .ThenInclude(p => p.Permission)
                    .Where(r => r.RoleCode == AnonymousRole.AnonymousRoleCode);

        }

        private IQueryable<Role> QueryRoleById(int roleId)
        {
            return _dbContext
                    .Roles
                    .AsNoTracking()
                    .Include(r => r.UserArea)
                    .Include(r => r.RolePermissions)
                    .ThenInclude(p => p.Permission)
                    .Where(r => r.RoleId == roleId);
        }

        #endregion
    }
}
