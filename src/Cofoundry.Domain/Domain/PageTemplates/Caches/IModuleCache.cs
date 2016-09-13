using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IModuleCache
    {
        PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter);

        Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter);
        
        void Clear();
    }
}
