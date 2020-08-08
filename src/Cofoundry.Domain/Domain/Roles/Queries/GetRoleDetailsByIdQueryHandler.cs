using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetRoleDetailsByIdQueryHandler 
        : IQueryHandler<GetRoleDetailsByIdQuery, RoleDetails>
        , IPermissionRestrictedQueryHandler<GetRoleDetailsByIdQuery, RoleDetails>
    {
        #region constructor

        private readonly IInternalRoleRepository _internalRoleRepository;

        public GetRoleDetailsByIdQueryHandler(
            IInternalRoleRepository internalRoleRepository
            )
        {
            _internalRoleRepository = internalRoleRepository;
        }

        #endregion

        #region execution

        public Task<RoleDetails> ExecuteAsync(GetRoleDetailsByIdQuery query, IExecutionContext executionContext)
        {
            return _internalRoleRepository.GetByIdAsync(query.RoleId);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetRoleDetailsByIdQuery command)
        {
            // Ignore permission for anonymous role.
            if (command.RoleId.HasValue)
            {
                yield return new RoleReadPermission();
            }
        }

        #endregion
    }
}
