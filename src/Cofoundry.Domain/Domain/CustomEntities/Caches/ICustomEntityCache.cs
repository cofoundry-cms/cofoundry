using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface ICustomEntityCache
    {
        Task<CustomEntityRoute[]> GetOrAddAsync(string customEntityTypeCode, Func<Task<CustomEntityRoute[]>> getter);
        CustomEntityRoute[] GetOrAdd(string customEntityTypeCode, Func<CustomEntityRoute[]> getter);
        
        void Clear();
        void Clear(string customEntityTypeCode, int customEntityId);
        void ClearRoutes(string customEntityTypeCode);
    }
}
