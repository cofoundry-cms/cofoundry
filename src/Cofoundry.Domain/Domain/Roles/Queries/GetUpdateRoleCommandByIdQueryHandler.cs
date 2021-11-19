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
        private readonly IInternalRoleRepository _internalRoleRepository;

        public GetUpdateRoleCommandByIdQueryHandler(
            IInternalRoleRepository internalRoleRepository
            )
        {
            _internalRoleRepository = internalRoleRepository;
        }

        public async Task<UpdateRoleCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdateRoleCommand> query, IExecutionContext executionContext)
        {
            var role = await _internalRoleRepository.GetByIdAsync(query.Id);
            if (role == null) return null;

            var command = new UpdateRoleCommand()
            {
                RoleId = role.RoleId,
                Title = role.Title
            };

            command.Permissions = role
                .Permissions
                .Select(p => new PermissionCommandData()
                {
                    EntityDefinitionCode = p.GetUniqueIdentifier(),
                    PermissionCode = p is IEntityPermission ? ((IEntityPermission)p).EntityDefinition.EntityDefinitionCode : null
                })
                .ToArray();

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdateRoleCommand> command)
        {
            yield return new RoleReadPermission();
        }
    }
}
