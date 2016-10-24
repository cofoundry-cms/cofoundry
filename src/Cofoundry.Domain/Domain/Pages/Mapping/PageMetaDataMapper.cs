using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageMetaDataMapper : IPageMetaDataMapper
    {
        private static readonly char[] SPLIT_CHARS = new char[] { ',' };
        private static readonly char[] TRIM_CHARS = new char[] { ',', ' ' };

        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public PageMetaDataMapper(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Merges meta for an individual page with the sitewide
        /// meta data settings.
        /// </summary>
        public void MergeSitewideData(PageMetaData metaData)
        {
            var splitKeywords = GetSiteMetaKeywords();
            MapMetaData(metaData, splitKeywords);
        }

        /// <summary>
        /// Merges meta for multiple pages with the sitewide
        /// meta data settings.
        /// </summary>
        public void MergeSitewideData(IEnumerable<PageMetaData> metaData)
        {
            var splitKeywords = GetSiteMetaKeywords();

            foreach (var md in metaData)
            {
                MapMetaData(md, splitKeywords);
            }
        }

        #endregion

        #region private helpers

        private string[] GetSiteMetaKeywords()
        {
            var siteMetaKeywords = _queryExecutor.Get<SeoSettings>().MetaKeywords ?? string.Empty;
            var splitKeywords = siteMetaKeywords.Split(SPLIT_CHARS);

            return splitKeywords;
        }

        private void MapMetaData(PageMetaData metaData, string[] siteKeywords)
        {
            metaData.Keywords = metaData.Keywords
                .Split(SPLIT_CHARS)
                .Union(siteKeywords)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Where(s => !string.IsNullOrEmpty(s))
                .Aggregate("", (a, b) => a.Trim() + ", " + b.Trim())
                .Trim(TRIM_CHARS);
        }

        #endregion
    }
}
