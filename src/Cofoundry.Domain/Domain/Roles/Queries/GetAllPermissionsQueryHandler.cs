using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllPermissionsQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<IPermission>, IEnumerable<IPermission>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<IPermission>, IEnumerable<IPermission>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsQueryHandler(
            IPermissionRepository permissionRepository
            )
        {
            _permissionRepository = permissionRepository;
        }

        public Task<IEnumerable<IPermission>> ExecuteAsync(GetAllQuery<IPermission> query, IExecutionContext executionContext)
        {
            var permissions =_permissionRepository
                .GetAll()
                .OrderBy(p => GetPrimaryOrdering(p))
                .AsEnumerable();

            return Task.FromResult(permissions);
        }

        private string GetPrimaryOrdering(IPermission permission)
        {
            if (permission is IEntityPermission) return ((IEntityPermission)permission).EntityDefinition.Name;

            return "ZZZ";
        }

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<IPermission> command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
