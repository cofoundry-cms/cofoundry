using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageGroupSummaryMapper"/>.
/// </summary>
[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroupSummaryMapper : IPageGroupSummaryMapper
{
    private readonly IAuditDataMapper _auditDataMapper;

    public PageGroupSummaryMapper(
        IAuditDataMapper auditDataMapper
        )
    {
        _auditDataMapper = auditDataMapper;
    }

    /// <inheritdoc/>
    public PageGroupSummary? Map(PageGroupSummaryQueryModel? queryModel)
    {
        var dbPageGroup = queryModel?.PageGroup;
        if (dbPageGroup == null || queryModel == null)
        {
            return null;
        }

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
