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
        /// Maps an EF PageVersion record from the db into an PageVersionSummary 
        /// object.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database.</param>
        public PageVersionSummary Map(PageVersion dbPageVersion)
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
