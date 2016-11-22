using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPageModuleTypeCache
    {
        PageModuleTypeSummary[] GetOrAdd(Func<PageModuleTypeSummary[]> getter);

        Task<PageModuleTypeSummary[]> GetOrAddAsync(Func<Task<PageModuleTypeSummary[]>> getter);

        Dictionary<string, PageModuleTypeFileLocation> GetOrAddFileLocations(Func<Dictionary<string, PageModuleTypeFileLocation>> getter);
        
        void Clear();
    }
}
