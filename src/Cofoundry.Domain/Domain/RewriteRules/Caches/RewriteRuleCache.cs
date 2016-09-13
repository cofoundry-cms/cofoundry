using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RewriteRuleCache : IRewriteRuleCache
    {
        #region constructor

        private const string REWRITERULESUMMARY_CACHEKEY = "RewriteRuleSummaries";
        private const string CACHEKEY = "COF_RewriteRules";

        private readonly IObjectCache _cache;
        public RewriteRuleCache(IObjectCacheFactory cacheFactory)
        {
            _cache = cacheFactory.Get(CACHEKEY);
        }

        #endregion

        #region public methods

        public RewriteRuleSummary[] GetOrAdd(Func<RewriteRuleSummary[]> getter)
        {
            return _cache.GetOrAdd(REWRITERULESUMMARY_CACHEKEY, getter);
        }

        public async Task<RewriteRuleSummary[]> GetOrAddAsync(Func<Task<RewriteRuleSummary[]>> getter)
        {
            return await _cache.GetOrAddAsync(REWRITERULESUMMARY_CACHEKEY, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        #endregion
    }
}
