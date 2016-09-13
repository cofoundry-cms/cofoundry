using System;
using System.Collections;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public interface IWebDirectoryCache
    {
        WebDirectoryRoute[] GetOrAdd(Func<WebDirectoryRoute[]> getter);
        
        void Clear();
    }
}
