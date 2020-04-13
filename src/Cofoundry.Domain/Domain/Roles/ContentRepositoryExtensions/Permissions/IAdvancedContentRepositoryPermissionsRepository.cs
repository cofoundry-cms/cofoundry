using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for permissions in the Cofoundry identity system.
    /// </summary>
    public interface IAdvancedContentRepositoryPermissionsRepository
    {
        #region queries

        /// <summary>
        /// Returns all IPermission instances registered with Cofoundry.
        /// </summary>
        IAdvancedContentRepositoryGetAllPermissionsQueryBuilder GetAll();

        #endregion
    }
}
