using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
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
        /// Maps a collection EF PageVersion records for a single page into 
        /// a collection of PageVersionSummary objects.
        /// </summary>
        /// <param name="pageId">Id of the page that these versions belong to.</param>
        /// <param name="dbVersions">PageVersion records from the database to map.</param>
        public List<PageVersionSummary> MapVersions(int pageId, ICollection<PageVersion> dbVersions)
        {
            if (dbVersions == null) throw new ArgumentNullException(nameof(dbVersions));
            if (pageId <= 0) throw new ArgumentOutOfRangeException(nameof(pageId));

            var orderedVersions = dbVersions
                .Select(MapVersion)
                .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Draft)
                .ThenByDescending(v => v.AuditData.CreateDate)
                .ToList();

            bool hasLatestPublishVersioned = false;
            var results = new List<PageVersionSummary>(dbVersions.Count);

            foreach (var dbVersion in dbVersions
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate))
            {
                if (dbVersion.PageId != pageId)
                {
                    var msg = $"Invalid PageId. PageVersionSummaryMapper.MapVersions is designed to map versions for a single custom entity only. Excepted PageId {pageId} got {dbVersion.PageId}";
                    throw new Exception(msg);
                }

                var result = MapVersion(dbVersion);
                if (!hasLatestPublishVersioned && result.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    result.IsLatestPublishedVersion = true;
                    hasLatestPublishVersioned = true;
                }

                results.Add(result);
            }

            return results;

        }

        private PageVersionSummary MapVersion(PageVersion dbPageVersion)
        {
            var result = new PageVersionSummary()
            {
                PageVersionId = dbPageVersion.PageVersionId,
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
