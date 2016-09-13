using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPageCache
    {
        Task<PageRoute[]> GetOrAddAsync(Func<Task<PageRoute[]>> getter);
        PageRoute[] GetOrAdd(Func<PageRoute[]> getter);
        
        void Clear();
        void Clear(int pageId);
        void ClearRoutes();
    }
}
