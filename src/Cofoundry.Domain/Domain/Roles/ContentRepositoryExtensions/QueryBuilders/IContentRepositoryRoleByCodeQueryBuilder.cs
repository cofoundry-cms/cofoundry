using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a role with the specified role code, returning null if the role
    /// could not be found. Roles only have a RoleCode if they have been generated 
    /// from code rather than the GUI. For GUI generated roles use a 'get by id' 
    /// query.
    /// </summary>
    public interface IContentRepositoryRoleByCodeQueryBuilder
    {
        /// <summary>
        /// RoleDetails is a fully featured role projection including all properties and 
        /// permission information.
        /// </summary>
        IDomainRepositoryQueryContext<RoleDetails> AsDetails();
    }
}
