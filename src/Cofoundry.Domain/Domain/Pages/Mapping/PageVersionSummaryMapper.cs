using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageVersionSummary objects.
    /// </summary>
    public class PageVersionSummaryMapper : IPageVersionSummaryMapper
    {
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
        private readonly IAuditDataMapper _auditDataMapper;

        public PageVersionSummaryMapper(
            IPageTemplateMicroSummaryMapper pageTemplateMapper,
            IAuditDataMapper auditDataMapper
            )
        {
            _pageTemplateMapper = pageTemplateMapper;
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// Maps a set of paged EF PageVersion records for a single page into 
        /// PageVersionSummary objects.
        /// </summary>
        /// <param name="pageId">Id of the page that these versions belong to.</param>
        /// <param name="dbResult">Paged result set of records to map.</param>
        public virtual PagedQueryResult<PageVersionSummary> MapVersions(int pageId, PagedQueryResult<PageVersion> dbResult)
        {
            if (dbResult == null) throw new ArgumentNullException(nameof(dbResult));
            if (pageId <= 0) throw new ArgumentOutOfRangeException(nameof(pageId));

            // We should only check for the latested published version on the first page
            // as it will only be 1st or 2nd in the list (depending on whether there is a draft)
            bool hasLatestPublishVersion = dbResult.PageNumber > 1;
            var results = new List<PageVersionSummary>(dbResult.Items.Count);

            foreach (var dbVersion in dbResult.Items.OrderByLatest())
            {
                if (dbVersion.PageId != pageId)
                {
                    var msg = $"Invalid PageId. PageVersionSummaryMapper.MapVersions is designed to map versions for a single custom entity only. Excepted PageId {pageId} got {dbVersion.PageId}";
                    throw new Exception(msg);
                }

                var result = MapVersion(dbVersion);
                if (!hasLatestPublishVersion && result.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    result.IsLatestPublishedVersion = true;
                    hasLatestPublishVersion = true;
                }

                results.Add(result);
            }

            return dbResult.ChangeType(results);

        }

        protected PageVersionSummary MapVersion(PageVersion dbPageVersion)
        {
            var result = new PageVersionSummary()
            {
                PageVersionId = dbPageVersion.PageVersionId,
                DisplayVersion = dbPageVersion.DisplayVersion,
                ShowInSiteMap = !dbPageVersion.ExcludeFromSitemap,
                Title = dbPageVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId
            };

            result.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);
            result.AuditData = _auditDataMapper.MapCreateAuditData(dbPageVersion);

            return result;
        }
    }
}
