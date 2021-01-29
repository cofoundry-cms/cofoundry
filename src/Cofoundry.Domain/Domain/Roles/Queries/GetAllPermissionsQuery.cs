using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all IPermission instances registered with Cofoundry.
    /// </summary>
    public class GetAllPermissionsQuery : IQuery<ICollection<IPermission>>
    {
    }
}
