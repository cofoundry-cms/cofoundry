using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetUpdateRoleCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateRoleCommand>, UpdateRoleCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdateRoleCommand>, UpdateRoleCommand>
    {
        #region constructor

        private readonly IInternalRoleRepository _internalRoleRepository;

        public GetUpdateRoleCommandByIdQueryHandler(
            IInternalRoleRepository internalRoleRepository
            )
        {
            _internalRoleRepository = internalRoleRepository;
        }

        #endregion

        #region execution

        public async Task<UpdateRoleCommand> ExecuteAsync(GetByIdQuery<UpdateRoleCommand> query, IExecutionContext executionContext)
        {
            var role = await _internalRoleRepository.GetByIdAsync(query.Id);
            var command = Mapper.Map<UpdateRoleCommand>(role);

            return command;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdateRoleCommand> command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
