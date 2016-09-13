using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IRewriteRuleCache
    {
        void Clear();
        RewriteRuleSummary[] GetOrAdd(Func<RewriteRuleSummary[]> getter);
        Task<RewriteRuleSummary[]> GetOrAddAsync(Func<Task<RewriteRuleSummary[]>> getter);
    }
}
