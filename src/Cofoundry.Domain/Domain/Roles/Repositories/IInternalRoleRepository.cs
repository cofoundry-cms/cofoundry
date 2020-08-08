using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Internal repository for fetching roles which bypasses CQS and permissions infrastructure
    /// to avoid circular references. Not to be used outside of Cofoundry core projects
    /// </summary>
    /// <remarks>
    /// Not actually marked internal due to internal visibility restrictions and dependency injection
    /// </remarks>
    public interface IInternalRoleRepository
    {
        RoleDetails GetById(int? roleId);
        Task<RoleDetails> GetByIdAsync(int? roleId);
    }
}
