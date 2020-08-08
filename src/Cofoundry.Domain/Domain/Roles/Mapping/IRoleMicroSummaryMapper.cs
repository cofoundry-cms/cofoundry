using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to RoleMicroSummary objects.
    /// </summary>
    public interface IRoleMicroSummaryMapper
    {
        /// <summary>
        /// Maps an EF Role record from the db into an RoleDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbRole">Role record from the database.</param>
        RoleMicroSummary Map(Role dbRole);
    }
}
