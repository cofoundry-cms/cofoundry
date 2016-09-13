using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using AutoMapper;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Aggregates given keywords with the site-wide pre-defined meta keywords
    /// </summary>
    public class SiteKeywordService : ISiteKeywordService
    {
        private static string[] EXCLUDE_LIST = new string[] { "a", "as", "an", "out", "in", "of", "and", "&", "the", "with", "or", "on", "in", "at", "to", "for", "is", "it", "are", "our", "we", "him", "he", "her", "she" };

        private readonly IQueryExecutor _queryExecutor;

        public SiteKeywordService(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Merges the specified string of keywords with the global site keywords
        /// </summary>
        public string Merge(string keywordsToMerge)
        {
            var settings = _queryExecutor.Get<SeoSettings>();
            var siteKeywords = settings.MetaKeywords ?? string.Empty;

            return (string.Empty + keywordsToMerge).Split(new char[] { ',', ' ', '-', '-', ':', ';', '_', '.', '?', '!' })
                .Union(siteKeywords.Split(new char[] { ',' }))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Where(s => !string.IsNullOrEmpty(s) && !EXCLUDE_LIST.Contains(s.ToLower()))
                .Aggregate(string.Empty, (a, b) => a.Trim() + "," + b.Trim())
                .Trim(new char[] { ',' });
        }
    }
}
