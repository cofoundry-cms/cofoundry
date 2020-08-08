using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Collections.ObjectModel;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Find a role with the specified role code, returning
    /// a RoleDetails object if one is found, otherwise null. Roles only
    /// have a RoleCode if they have been generated from code
    /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
    /// </summary>
    public class GetRoleDetailsByRoleCodeQueryHandler
        : IQueryHandler<GetRoleDetailsByRoleCodeQuery, RoleDetails>
        , IPermissionRestrictedQueryHandler<GetRoleDetailsByRoleCodeQuery, RoleDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IInternalRoleRepository _internalRoleRepository;
        private readonly IRoleCache _roleCache;

        public GetRoleDetailsByRoleCodeQueryHandler(
            CofoundryDbContext dbContext,
            IInternalRoleRepository internalRoleRepository,
            IRoleCache roleCache
            )
        {
            _dbContext = dbContext;
            _internalRoleRepository = internalRoleRepository;
            _roleCache = roleCache;
        }

        #endregion

        #region execution

        public async Task<RoleDetails> ExecuteAsync(GetRoleDetailsByRoleCodeQuery query, IExecutionContext executionContext)
        {
            var roleCodeLookup = await _roleCache.GetOrAddRoleCodeLookupAsync(async () =>
            {
                var roleCodes = await QueryRoleCodes()
                    .ToDictionaryAsync(r => r.RoleCode, r => r.RoleId);

                return new ReadOnlyDictionary<string, int>(roleCodes);
            });

            var id = roleCodeLookup.GetOrDefault(query.RoleCode);
            if (id <= 0) return null;

            return await _internalRoleRepository.GetByIdAsync(id);
        }

        private IQueryable<Role> QueryRoleCodes()
        {
            return _dbContext
                    .Roles
                    .AsNoTracking()
                    .Where(r => r.RoleCode != null);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetRoleDetailsByRoleCodeQuery command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
