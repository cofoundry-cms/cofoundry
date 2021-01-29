using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdateRoleCommandByIdQueryHandler 
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdateRoleCommand>, UpdateRoleCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdateRoleCommand>, UpdateRoleCommand>
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

        public async Task<UpdateRoleCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdateRoleCommand> query, IExecutionContext executionContext)
        {
            var role = await _internalRoleRepository.GetByIdAsync(query.Id);
            var command = new UpdateRoleCommand()
            {
                RoleId = role.RoleId,
                Title = role.Title
            };

            command.Permissions = role
                .Permissions
                .Select(p => new PermissionCommandData()
                {
                    EntityDefinitionCode = p.GetUniqueCode(),
                    PermissionCode = p is IEntityPermission ? ((IEntityPermission)p).EntityDefinition.EntityDefinitionCode : null
                })
                .ToArray();

            return command;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdateRoleCommand> command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
