using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IRoleCache
    {
        RoleDetails GetOrAdd(int roleId, Func<RoleDetails> getter);
        Task<RoleDetails> GetOrAddAsync(int roleId, Func<Task<RoleDetails>> getter);
        RoleDetails GetOrAddAnonymousRole(Func<RoleDetails> getter);
        Task<RoleDetails> GetOrAddAnonymousRoleAsync(Func<Task<RoleDetails>> getter);

        void Clear();
        void Clear(int roleId);
    }
}
