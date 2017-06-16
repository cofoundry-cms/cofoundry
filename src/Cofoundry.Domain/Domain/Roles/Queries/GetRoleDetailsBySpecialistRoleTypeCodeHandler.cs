using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Collections.ObjectModel;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Find a role with the specified specialist role type code, returning
    /// a RoleDetails object if one is found, otherwise null. Roles only
    /// have a SpecialistRoleTypeCode if they have been generated from code
    /// rather than the GUI. For GUI generated roles use GetRoleDetailsByIdQuery.
    /// </summary>
    public class GetRoleDetailsBySpecialistRoleTypeCodeHandler
        : IQueryHandler<GetRoleDetailsBySpecialistRoleTypeCode, RoleDetails>
        , IAsyncQueryHandler<GetRoleDetailsBySpecialistRoleTypeCode, RoleDetails>
        , IPermissionRestrictedQueryHandler<GetRoleDetailsBySpecialistRoleTypeCode, RoleDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IInternalRoleRepository _internalRoleRepository;
        private readonly IRoleCache _roleCache;

        public GetRoleDetailsBySpecialistRoleTypeCodeHandler(
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

        public RoleDetails Execute(GetRoleDetailsBySpecialistRoleTypeCode query, IExecutionContext executionContext)
        {
            var roleCodeLookup = _roleCache.GetOrAddSpecialistRoleTypeCodeLookup(() =>
            {
                var roleCodes = QueryRoleCodes()
                    .ToDictionary(r => r.SpecialistRoleTypeCode, r => r.RoleId);

                return new ReadOnlyDictionary<string, int>(roleCodes);
            });

            var id = roleCodeLookup.GetOrDefault(query.SpecialistRoleTypeCode);
            if (id <= 0) return null;

            return _internalRoleRepository.GetById(id);
        }

        public async Task<RoleDetails> ExecuteAsync(GetRoleDetailsBySpecialistRoleTypeCode query, IExecutionContext executionContext)
        {
            var roleCodeLookup = await _roleCache.GetOrAddSpecialistRoleTypeCodeLookupAsync(async () =>
            {
                var roleCodes = await QueryRoleCodes()
                    .ToDictionaryAsync(r => r.SpecialistRoleTypeCode, r => r.RoleId);

                return new ReadOnlyDictionary<string, int>(roleCodes);
            });

            var id = roleCodeLookup.GetOrDefault(query.SpecialistRoleTypeCode);
            if (id <= 0) return null;

            return await _internalRoleRepository.GetByIdAsync(id);
        }

        private IQueryable<Role> QueryRoleCodes()
        {
            return _dbContext
                    .Roles
                    .AsNoTracking()
                    .Where(r => r.SpecialistRoleTypeCode != null);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetRoleDetailsBySpecialistRoleTypeCode command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
