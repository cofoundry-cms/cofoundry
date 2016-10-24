using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public interface IPageMetaDataMapper
    {
        /// <summary>
        /// Merges meta for an individual page with the sitewide
        /// meta data settings.
        /// </summary>
        void MergeSitewideData(PageMetaData metaData);

        /// <summary>
        /// Merges meta for multiple pages with the sitewide
        /// meta data settings.
        /// </summary>
        void MergeSitewideData(IEnumerable<PageMetaData> metaData);
    }
}
