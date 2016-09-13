using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface ILocaleCache
    {
        ActiveLocale[] GetOrAdd(Func<ActiveLocale[]> getter);
        Task<ActiveLocale[]> GetOrAddAsync(Func<Task<ActiveLocale[]>> getter);
    }
}
