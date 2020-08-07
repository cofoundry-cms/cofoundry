using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a role by it's database id, returning null if the role could 
    /// not be found. If no role id is specified in the query then the anonymous 
    /// role is returned.
    /// </summary>
    public interface IContentRepositoryRoleByIdQueryBuilder
    {
        /// <summary>
        /// RoleDetails is a fully featured role projection including all properties and 
        /// permission information.
        /// </summary>
        IDomainRepositoryQueryContext<RoleDetails> AsDetails();
    }
}
