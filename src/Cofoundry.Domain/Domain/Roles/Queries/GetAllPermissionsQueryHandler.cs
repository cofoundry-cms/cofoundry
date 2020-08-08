using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns all IPermission instances registered with Cofoundry.
    /// </summary>
    public class GetAllPermissionsQueryHandler 
        : IQueryHandler<GetAllPermissionsQuery, ICollection<IPermission>>
        , IPermissionRestrictedQueryHandler<GetAllPermissionsQuery, ICollection<IPermission>>
    {
        #region constructor

        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsQueryHandler(
            IPermissionRepository permissionRepository
            )
        {
            _permissionRepository = permissionRepository;
        }

        #endregion

        #region execution

        public Task<ICollection<IPermission>> ExecuteAsync(GetAllPermissionsQuery query, IExecutionContext executionContext)
        {
            var permissions =_permissionRepository
                .GetAll()
                .OrderBy(p => GetPrimaryOrdering(p))
                .ToList();

            return Task.FromResult< ICollection<IPermission>>(permissions);
        }

        private string GetPrimaryOrdering(IPermission permission)
        {
            if (permission is IEntityPermission) return ((IEntityPermission)permission).EntityDefinition.Name;

            return "ZZZ";
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPermissionsQuery command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
