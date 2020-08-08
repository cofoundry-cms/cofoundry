using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageGroupSummary objects.
    /// </summary>
    public class PageGroupSummaryMapper : IPageGroupSummaryMapper
    {
        private readonly IAuditDataMapper _auditDataMapper;

        public PageGroupSummaryMapper(
            IAuditDataMapper auditDataMapper
            )
        {
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// Maps an EF PageGroup record from the db into an PageGroupSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="queryModel">Query model with data from the database.</param>
        public PageGroupSummary Map(PageGroupSummaryQueryModel queryModel)
        {
            var dbPageGroup = queryModel?.PageGroup;
            if (dbPageGroup == null) return null;

            var pageGroup = new PageGroupSummary()
            {
                Name = dbPageGroup.GroupName,
                PageGroupId = dbPageGroup.PageGroupId,
                ParentGroupId = dbPageGroup.ParentGroupId,
                NumPages = queryModel.NumPages,
                AuditData = _auditDataMapper.MapCreateAuditData(dbPageGroup)
            };

            return pageGroup;
        }
    }
}
