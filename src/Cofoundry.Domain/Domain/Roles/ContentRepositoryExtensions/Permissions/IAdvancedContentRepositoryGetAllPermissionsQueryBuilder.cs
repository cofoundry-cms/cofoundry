using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all IPermission instances registered with Cofoundry.
    /// </summary>
    public interface IAdvancedContentRepositoryGetAllPermissionsQueryBuilder
    {
        /// <summary>
        /// Returns all the permissions in the system as instances
        /// of their base definition types, which all inherit from IPermission.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<IPermission>> AsIPermission();
    }
}
